using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LinqCollectionPref
{
    class Program
    {
        const int ITR = 20__000;
        static Customer[] customers;
        static CustomerPreference[] customerPreferences;
        static List<CustomerAggregate> customerAggregates;
        static Dictionary<int, CustomerPreference> customerPreferencesDict;

        private static void JoinWithForLoop()
        {
            customerAggregates = new List<CustomerAggregate>();
            for (int i = 0; i < customers.Length; i++)
            {
                var cust = customers[i];
                var preference = customerPreferences.SingleOrDefault(ttl => ttl.CustomerID == cust.CustomerID);
                customerAggregates.Add(new CustomerAggregate
                {
                    CustomerID = cust.CustomerID,
                    Name = cust.Name,
                    Prefernce = preference
                });
            }
        }

        private static void JoinWithForeachLoop()
        {
            customerAggregates = new List<CustomerAggregate>();
            foreach (Customer cust in customers)
            {
                var preference = customerPreferences.SingleOrDefault(ttl => ttl.CustomerID == cust.CustomerID);
                customerAggregates.Add(new CustomerAggregate
                {
                    CustomerID = cust.CustomerID,
                    Name = cust.Name,
                    Prefernce = preference
                });
            }
        }

        private static void JoinWithSelect()
        {
            customerAggregates = customers.Select(cust =>
                                            {
                                                var preference = customerPreferences.SingleOrDefault(ttl => ttl.CustomerID == cust.CustomerID);
                                                return new CustomerAggregate
                                                {
                                                    CustomerID = cust.CustomerID,
                                                    Name = cust.Name,
                                                    Prefernce = preference
                                                };
                                            }).ToList();
        }

        private static void JoinWithJoin()
        {
            customerAggregates = customers.Join(customerPreferences,
                                                cust => cust.CustomerID,
                                                custPref => custPref.CustomerID,
                                                (cust, custPref) => new CustomerAggregate
                                                {
                                                    CustomerID = cust.CustomerID,
                                                    Name = cust.Name,
                                                    Prefernce = custPref
                                                }).ToList();
        }

        private static void JoinWithQueryJoin()
        {
            customerAggregates = (from customer in customers
                                  join preference in customerPreferences on customer.CustomerID equals preference.CustomerID
                                  select new CustomerAggregate
                                  {
                                      CustomerID = customer.CustomerID,
                                      Name = customer.Name,
                                      Prefernce = preference
                                  }).ToList();
        }

        private static void JoinWithDict()
        {
            var custDict = customerPreferences.ToDictionary(k => k.CustomerID);
            customerAggregates = customers.Select(customer =>
            {
                var preference = custDict[customer.CustomerID];
                return new CustomerAggregate
                {
                    CustomerID = customer.CustomerID,
                    Name = customer.Name,
                    Prefernce = preference
                };
            }).ToList();

        }

        private static void JoinWithDictInner()
        {
            customerAggregates = customers.Select(customer =>
            {
                var preference = customerPreferencesDict[customer.CustomerID];
                return new CustomerAggregate
                {
                    CustomerID = customer.CustomerID,
                    Name = customer.Name,
                    Prefernce = preference
                };
            }).ToList();
        }

        private static void JoinWithManualIterationDict()
        {
            var preferences = new Dictionary<int, CustomerPreference>(customerPreferences.Length);
            foreach (var custPref in customerPreferences)
            {
                preferences.Add(custPref.CustomerID, custPref);
            }

            customerAggregates = new List<CustomerAggregate>(customers.Length);
            foreach (var customer in customers)
            {
                preferences.TryGetValue(customer.CustomerID, out var preference);
                customerAggregates.Add(new CustomerAggregate
                {
                    CustomerID = customer.CustomerID,
                    Name = customer.Name,
                    Prefernce = preference
                });
            }
        }

        private static void BasicSeed()
        {
            customers = Enumerable.Range(0, ITR)
                .Select(i => new Customer { CustomerID = i, Name = $"Javad {i}" })
                .ToArray();

            customerPreferences = Enumerable.Range(0, ITR)
                .Select(i => new CustomerPreference { CustomerID = i, Total = i * 2 })
                .ToArray();

            customerPreferencesDict = customerPreferences.ToDictionary(c => c.CustomerID);
        }

        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            BasicSeed();
            Console.WriteLine("start ...");
            stopwatch.Start();
            //JoinWithForLoop(); // 11335 mili Debug 6526 mili Release
            //JoinWithForeachLoop(); // 11279 mili Debug 6850 mili Release
            //JoinWithSelect(); //11680 mili Debug 5994 mili release
            //JoinWithJoin(); // Exccellent :D 129 mili Debug 129 mili Release ... there was no diff between release and debug ... the performance was sooo good :D
            JoinWithQueryJoin(); // same as above :D
            //JoinWithDict(); // Maybe a little bit slower ... but in general .. there was no diff than above
            //JoinWithDictInner(); // same as above :D
            //JoinWithManualIterationDict(); // excellent ... same as above :D
            stopwatch.Stop();

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
            Console.ReadKey();
        }
    }
}
