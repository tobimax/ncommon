using System.Linq;
using NCommon.Data.EntityFramework.Tests.OrdersDomain;
using NCommon.Extensions;
using NCommon.Specifications;
using NUnit.Framework;
using Rhino.Mocks;

namespace NCommon.Data.EntityFramework.Tests
{
    [TestFixture]
    public class EFRepositoryEagerFetchingTests : EFRepositoryTestBase
    {
        [Test]
        public void Can_eager_fetch()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Order order = null;
                Order savedOrder = null;

                testData.Batch(x => order = x.CreateOrderForCustomer(x.CreateCustomer()));

                using (var scope = new UnitOfWorkScope())
                {
                    savedOrder = new EFRepository<Order>()
                        .Fetch(o => o.Customer)
                        .Where(x => x.OrderID == order.OrderID)
                        .SingleOrDefault();
                    scope.Commit();
                }

                Assert.NotNull(savedOrder);
                Assert.NotNull(savedOrder.Customer);
                Assert.DoesNotThrow(() => { var firstName = savedOrder.Customer.FirstName; });
            }
        }
        
        [Test]
        public void Can_eager_fetch_many()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                });

                using (var scope = new UnitOfWorkScope())
                {
                    savedCustomer = new EFRepository<Customer>()
                        .FetchMany(x => x.Orders)
                        .ThenFetchMany(x => x.OrderItems)
                        .ThenFetch(x => x.Product)
                        .SingleOrDefault(x => x.CustomerID == customer.CustomerID);
                    scope.Commit();
                }

                Assert.NotNull(savedCustomer);
                Assert.NotNull(savedCustomer.Orders);
                savedCustomer.Orders.ForEach(order =>
                {
                    Assert.NotNull(order.OrderItems);
                    order.OrderItems.ForEach(orderItem => Assert.NotNull(orderItem.Product));
                });
            }
        }

        [Test]
        public void Can_eager_fetch_using_for_using_specification_and_fetching_strategy()
        {
            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                testData.Batch(actions =>
                {
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("PA", 2));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("DE", 5));
                    actions.CreateOrdersForCustomers(actions.CreateCustomersInState("LA", 3));
                });

                using (new UnitOfWorkScope())
                {


                    var customersInPa = new Specification<Order>(x => x.Customer.State == "DE");

                    var ordersRepository = new EFRepository<Order>();
                    IQueryable<Order> results = ordersRepository.Query(customersInPa);

                    Assert.That(results.Count(), Is.GreaterThan(0));
                    Assert.That(results.Count(), Is.EqualTo(5));
                }
            }
        }

        [Test]
        public void Can_eager_fetch_using_for_and_specification_()
        {
            Locator.Stub(x => x.GetAllInstances<IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>>())
                .Return(new[] { new FetchingStrategy() });

            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                });

                using (var scope = new UnitOfWorkScope())
                {
                    var createdCustomer = new Specification<Customer>(x => x.CustomerID == customer.CustomerID);

                    savedCustomer = new EFRepository<Customer>()
                        .QueryFor<EFRepositoryEagerFetchingTests>(createdCustomer)
                        .SingleOrDefault();
                    scope.Commit();
                }

                Assert.NotNull(savedCustomer);
                Assert.NotNull(savedCustomer.Orders);
                savedCustomer.Orders.ForEach(order =>
                {
                    Assert.NotNull(order.OrderItems);
                    order.OrderItems.ForEach(orderItem => Assert.NotNull(orderItem.Product));
                });
            }
        }


        [Test]
        public void Can_eager_fetch_using_for()
        {
            Locator.Stub(x => x.GetAllInstances<IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>>())
                .Return(new[] {new FetchingStrategy()});

            using (var testData = new EFTestData(OrdersContextProvider()))
            {
                Customer customer = null;
                Customer savedCustomer = null;
                testData.Batch(x =>
                {
                    customer = x.CreateCustomer();
                    var order = x.CreateOrderForCustomer(customer);
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                    order.OrderItems.Add(x.CreateItem(order, x.CreateProduct()));
                });

                using (var scope = new UnitOfWorkScope())
                {
                    savedCustomer = new EFRepository<Customer>()
                        .For<EFRepositoryEagerFetchingTests>()
                        .SingleOrDefault(x => x.CustomerID == customer.CustomerID);
                    scope.Commit();
                }

                Assert.NotNull(savedCustomer);
                Assert.NotNull(savedCustomer.Orders);
                savedCustomer.Orders.ForEach(order =>
                {
                    Assert.NotNull(order.OrderItems);
                    order.OrderItems.ForEach(orderItem => Assert.NotNull(orderItem.Product));
                });
            }
        }

        class FetchingStrategy : IFetchingStrategy<Customer, EFRepositoryEagerFetchingTests>
        {
            public IQueryable<Customer> Define(IRepository<Customer> repository)
            {
                return repository.FetchMany(x => x.Orders)
                    .ThenFetchMany(x => x.OrderItems)
                    .ThenFetch(x => x.Product);
            }
        }
    }
}