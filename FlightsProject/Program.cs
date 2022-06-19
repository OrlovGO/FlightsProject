using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightCodingTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            FlightBuilder TestRoutes = new FlightBuilder();
            Filter filter = new Filter();
            IList<Flight> TestList = new List<Flight>();
            TestList = TestRoutes.GetFlights();

            IList<Flight> Backup = TestList.ToList();

            filter.FlightOutput(TestList);

            Console.WriteLine("Что вы хотите сделать?");
            Console.WriteLine("Введите create чтобы добавить правило в список");
            Console.WriteLine("Введите del чтобы удалить правило из списка");
            Console.WriteLine("Введите exit, чтобы завершить работу");
            Console.WriteLine("");
            Console.WriteLine("Поддерживаемые команды для правил:");
            Console.WriteLine("DepartureTime > < = ЗНАЧ(Дней от текущ.)");
            Console.WriteLine("ArrivalTime > < = ЗНАЧ(Дней от текущ.)");
            Console.WriteLine("LandTime > < = ЗНАЧ(Время на земле, часов)");
            Console.WriteLine("ArrivalEarlier Y/N(Есть ли сегменты с прилетом раньше вылета)");

            bool stop = false;
            while (!stop)
            {
                string Option;
                Option = Console.ReadLine();
                if (Option == "create")
                {
                    TestList = Backup.ToList();
                    filter.CreateRule();
                    filter.ApplyFilter(TestList);
                    filter.RulesOutput();
                    filter.FlightOutput(TestList);
                }
                if (Option == "del")
                {
                    TestList = Backup.ToList();
                    filter.DeleteRule();
                    filter.ApplyFilter(TestList);
                    filter.RulesOutput();
                    filter.FlightOutput(TestList);
                }
                if (Option == "exit")
                {
                    stop = true;
                }
            }
 
        }
    }

    public class Filter
    {
        class Rule
        {
            public string RuleExpression;
            public string Parameter;
            public string Condition;
            public System.DateTime TargetTime;
            public int TargetValue;
            public bool IsCorrect(Flight Fly)
            {
                bool flag = false;
                if(Parameter == "DepartureTime")
                {
                    int CompareResult = DateTime.Compare(Fly.Segments[0].DepartureDate, TargetTime);
                    if (Condition == "<")
                    {
                        if (CompareResult < 0)
                            flag = true;
                    }
                    if (Condition == ">")
                    {
                        if (CompareResult > 0)
                            flag = true;
                    }
                    if (Condition == "=")
                    {
                        if (CompareResult == 0)
                            flag = true;
                    }
                }

                if (Parameter == "ArrivalTime")
                {
                    int CompareResult = DateTime.Compare(Fly.Segments[Fly.Segments.Count-1].ArrivalDate, TargetTime);
                    if (Condition == "<")
                    {
                        if (CompareResult < 0)
                            flag = true;
                    }
                    if (Condition == ">")
                    {
                        if (CompareResult > 0)
                            flag = true;
                    }
                    if (Condition == "=")
                    {
                        if (CompareResult == 0)
                            flag = true;
                    }
                }

                if (Parameter == "LandTime")
                {
                    int Duration = 0;
                    for (int j = Fly.Segments.Count() - 1; j >= 0; j--)
                    {
                        if (j != 0)
                        {
                            TimeSpan tmp = Fly.Segments[j].DepartureDate - Fly.Segments[j - 1].ArrivalDate;
                            Duration += Convert.ToInt32(tmp.TotalHours);
                        }

                    }

                    if (Condition == "<")
                    {
                        if (Duration < TargetValue)
                        {
                            flag = true;
                        }
                    }
                    if (Condition == ">")
                    {
                        if (Duration > TargetValue)
                        {
                            flag = true;
                        }
                    }
                    if (Condition == "=")
                    {
                        if (Duration == TargetValue)
                        {
                            flag = true;
                        }
                    }
                }

                if (Parameter == "ArrivalEarlier")
                {
                    int CompareResult = 0;
                    for (int j = Fly.Segments.Count() - 1; j >= 0; j--)
                    {
                        CompareResult = DateTime.Compare(Fly.Segments[j].ArrivalDate, Fly.Segments[j].DepartureDate);
                        if (Condition == "Y")
                        {
                            if (CompareResult < 0)
                                flag = true;
                        }
                        if (Condition == "N")
                        {
                            if (CompareResult >= 0)
                                flag = true;
                        }
                    }
                }
                return flag;
            }

           
        }

        List<Rule> Rules = new List<Rule>();
        public void ApplyFilter(IList<Flight> FlightList)
        {
            for (int i = FlightList.Count - 1; i >= 0; i--)
            {
                foreach(Rule r in Rules)
                {
                    if(r.IsCorrect(FlightList[i]) == false)
                    {
                        FlightList.Remove(FlightList[i]);
                        break;
                    }
                }
            }
        }

        public void CreateRule()
        {
            Rule rule = new Rule();
            bool error = false;
            Console.WriteLine("Введите правило:");
            rule.RuleExpression = Console.ReadLine();
            string[] RuleElements = rule.RuleExpression.Split(' ');
            if (RuleElements.Length == 3 || (RuleElements.Length == 2 && RuleElements[0] == "ArrivalEarlier"))
            {
                rule.Parameter = RuleElements[0];
                rule.Condition = RuleElements[1];
                if (rule.Parameter == "DepartureTime" || rule.Parameter == "ArrivalTime")
                {
                    int days;
                    days = Convert.ToInt32(RuleElements[2]);
                    rule.TargetTime = DateTime.Now.AddDays(days);
                }
                else if (rule.Parameter == "LandTime")
                {
                    rule.TargetValue = Convert.ToInt32(RuleElements[2]);
                }
                else if (rule.Parameter != "ArrivalEarlier")
                {
                    Console.WriteLine("Команда не распознана!");
                    error = true;
                }

                if (rule.Condition != "Y" && rule.Condition != "N" && rule.Condition != ">"
                    && rule.Condition != "<" && rule.Condition != "=")
                {
                    Console.WriteLine("Команда не распознана!");
                    error = true;
                }

                if (!error)
                {
                    Rules.Add(rule);
                }
            }
        }

        public void RulesOutput()
        {
            Console.WriteLine("Список правил:");
            foreach (Rule r in Rules)
            {
                Console.WriteLine(r.RuleExpression);
            }
        }
        public void DeleteRule()
        {
            Console.WriteLine("Какое правило удалить?");
            int num = 0;
            num = Convert.ToInt32(Console.ReadLine());
            Rules.Remove(Rules[num]);
        }
        public void FlightOutput(IList<Flight> FlightList)
        {
            Console.WriteLine("Текущий список полетов:");
            int FlightNum = 0;
            foreach (Flight Fly in FlightList)
            {
                FlightNum++;
                Console.WriteLine($"{"Полет "} {FlightNum}");
                for (int i = 0; i < Fly.Segments.Count(); i++)
                {
                    Console.Write($"{Fly.Segments[i].DepartureDate} {" "} {Fly.Segments[i].ArrivalDate}");
                    Console.WriteLine(" ");
                    if (i == Fly.Segments.Count() - 1)
                        Console.WriteLine("Конец полета");
                    else
                        Console.WriteLine("Пересадка...");
                }
                Console.WriteLine(" ");
            }

        }
    }
  
    public class FlightBuilder
    {
        private DateTime _threeDaysFromNow;

        public FlightBuilder()
        {
            _threeDaysFromNow = DateTime.Now.AddDays(3);
        }

        public IList<Flight> GetFlights()
        {
            return new List<Flight>
                       {
                           //A normal flight with two hour duration
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2)),

                           //A normal multi segment flight
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(5)),
                           
                           //A flight departing in the past
                           CreateFlight(_threeDaysFromNow.AddDays(-6), _threeDaysFromNow),

                           //A flight that departs before it arrives
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(-6)),

                           //A flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(5), _threeDaysFromNow.AddHours(6)),

                            //Another flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(4), _threeDaysFromNow.AddHours(6), _threeDaysFromNow.AddHours(7))
                       };
        }

        private static Flight CreateFlight(params DateTime[] dates)
        {
            if (dates.Length % 2 != 0) throw new ArgumentException("You must pass an even number of dates,", "dates");

            var departureDates = dates.Where((date, index) => index % 2 == 0);
            var arrivalDates = dates.Where((date, index) => index % 2 == 1);

            var segments = departureDates.Zip(arrivalDates,
                                              (departureDate, arrivalDate) =>
                                              new Segment { DepartureDate = departureDate, ArrivalDate = arrivalDate }).ToList();

            return new Flight { Segments = segments };
        }
    }

    public class Flight
    {
        public IList<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    }
}