using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewspaperSellerModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DayTypeDistributions = new List<DayTypeDistribution>();
            DemandDistributions = new List<DemandDistribution>();
            SimulationTable = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }
        ///////////// INPUTS /////////////
        public int NumOfNewspapers { get; set; }
        public int NumOfRecords { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ScrapPrice { get; set; }
        public decimal UnitProfit { get; set; }
        public List<DayTypeDistribution> DayTypeDistributions { get; set; }
        public List<DemandDistribution> DemandDistributions { get; set; }

        ///////////// OUTPUTS /////////////
        public List<SimulationCase> SimulationTable { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }
        
        SimulationSystem simulationsystem;
         public void Day_type_distribution()
        {
            List<DayTypeDistribution> Day_types = simulationsystem.DayTypeDistributions;
            Day_types[0].CummProbability = Day_types[0].Probability;
            Day_types[0].MinRange = 1;
            Day_types[0].MaxRange = (int)(Day_types[0].CummProbability * 100); // may cause casting problems 

            for(int i=1;i<Day_types.Count;i++)
            {
                Day_types[i].CummProbability = Day_types[i-1].CummProbability+Day_types[i].Probability;
                Day_types[i].MinRange = Day_types[i-1].MinRange + 1;
                Day_types[i].MaxRange = (int)(Day_types[i].CummProbability * 100); // may cause casting problems 
            }
            simulationsystem.DayTypeDistributions = Day_types;
        }

        public void Demand_distribution()
        {
            // the first row is calculated manually 
            
            List<DemandDistribution> Demands = simulationsystem.DemandDistributions;
            for(int i=0;i<3;i++)
            {
                Demands[0].DayTypeDistributions[i].CummProbability = Demands[0].DayTypeDistributions[i].Probability;
                Demands[0].DayTypeDistributions[i].MinRange = 1;
                Demands[0].DayTypeDistributions[i].MaxRange = (int)Demands[0].DayTypeDistributions[i].CummProbability * 100;

            }
            /*
             * i will leave this and remove after testing just to double check the calc because i am so sleepy right now 
             * 
                Demands[0].DayTypeDistributions[0].CummProbability = Demands[0].DayTypeDistributions[0].Probability;
                Demands[0].DayTypeDistributions[1].CummProbability = Demands[0].DayTypeDistributions[1].Probability;
                Demands[0].DayTypeDistributions[2].CummProbability = Demands[0].DayTypeDistributions[2].Probability;

                Demands[0].DayTypeDistributions[0].MinRange = 1;
                Demands[0].DayTypeDistributions[1].MinRange = 1;
                Demands[0].DayTypeDistributions[2].MinRange = 1;

                Demands[0].DayTypeDistributions[0].MaxRange = (int)Demands[0].DayTypeDistributions[0].CummProbability*100;
                Demands[0].DayTypeDistributions[1].MaxRange = (int)Demands[0].DayTypeDistributions[1].CummProbability * 100;
                Demands[0].DayTypeDistributions[2].MaxRange = (int)Demands[0].DayTypeDistributions[2].CummProbability * 100;
                */
            //after finishing you can make it in a loop with demands[0] and then loop from 1 to 3 
            
            for (int i=1;i<Demands.Count;i++)
            {
                //outer loops iterate over all demands -> row wise iteration  
                for(int j=0;j<3;j++)
                {
                    //inner loop iterate over all the 3 possibilites -> column wise iteration
                    Demands[i].DayTypeDistributions[j].CummProbability = Demands[i].DayTypeDistributions[j].Probability+Demands[i-1].DayTypeDistributions[j].CummProbability;
                    Demands[i].DayTypeDistributions[j].MinRange = Demands[i-1].DayTypeDistributions[j].MaxRange+1;
                    Demands[i].DayTypeDistributions[j].MaxRange = (int)Demands[i-1].DayTypeDistributions[j].CummProbability * 100;
                }
            }
            simulationsystem.DemandDistributions = Demands;
        }

        public void Sim_Table()
        {
            int Min = 1;
            int Max = 100;
            int Day = 1;
            Random random = new Random();
            while(Day<=simulationsystem.NumOfRecords)
            {
                SimulationCase simulationCase = new SimulationCase(); 
                //for each day we need 
                //day type , demand and profit 

                // random values which the selection is based on  
                int Random_Day = random.Next(Min, Max);
                int Random_Demand = random.Next(Min, Max);

                simulationCase.RandomNewsDayType = Random_Day;
                simulationCase.RandomDemand = Random_Demand;
                simulationCase.DayNo = Day;
                Day++;

                //loop to select daytype
                for(int i=0;i<simulationsystem.DayTypeDistributions.Count;i++)
                {
                    if(Random_Day>=simulationsystem.DayTypeDistributions[i].MinRange&& Random_Day <= simulationsystem.DayTypeDistributions[i].MaxRange)
                    {
                        simulationCase.NewsDayType = simulationsystem.DayTypeDistributions[i].DayType;
                        break;
                    }
                }

                //loop to select demand

                for (int i = 0; i < simulationsystem.DemandDistributions.Count; i++)
                {
                    //you have the day type as an input so the column will be fixed and you loop rowswise to check ranges till
                    int Type = (int)simulationCase.NewsDayType; // how to use the fucking enum and remove the case  
                    int Temp_Min = simulationsystem.DemandDistributions[i].DayTypeDistributions[Type].MinRange;
                    int Temp_Max = simulationsystem.DemandDistributions[i].DayTypeDistributions[Type].MaxRange;
                    if (Temp_Min<=Random_Demand&&  Temp_Max<= Random_Demand)
                    {
                        //you can create two variables for the min and max before the condition to make the condition easier to understand 

                        simulationCase.Demand = simulationsystem.DemandDistributions[i].Demand;
                        break;
                    }

                }
                // Calculate profit

                //  !!!!!!!!!check the calculations again after the gui cause i was a bit sleepy!!!!!!!!!! 
                decimal profit = simulationsystem.SellingPrice - simulationsystem.PurchasePrice;
                simulationCase.DailyCost = simulationsystem.NumOfNewspapers * simulationsystem.PurchasePrice;
                simulationCase.SalesProfit = Math.Min(simulationCase.Demand, simulationsystem.NumOfNewspapers) * simulationsystem.SellingPrice;
                simulationCase.LostProfit = Math.Max(0, (simulationCase.Demand - simulationsystem.NumOfNewspapers) * profit);
                simulationCase.ScrapProfit = Math.Max(0, (simulationsystem.NumOfNewspapers - simulationCase.Demand) * simulationsystem.ScrapPrice);
                simulationCase.DailyNetProfit = simulationCase.SalesProfit - simulationCase.LostProfit - simulationCase.DailyCost + simulationCase.ScrapProfit;


                //calculate performance meausures
                simulationsystem.PerformanceMeasures.TotalCost += simulationCase.DailyCost;
                simulationsystem.PerformanceMeasures.TotalSalesProfit += simulationCase.SalesProfit;
                simulationsystem.PerformanceMeasures.TotalLostProfit += simulationCase.LostProfit;
                simulationsystem.PerformanceMeasures.TotalScrapProfit += simulationCase.ScrapProfit;
                simulationsystem.PerformanceMeasures.TotalNetProfit += simulationCase.DailyNetProfit;

                //calculate days with more demand and days with unsold papers
                // from the table i deduced that if the lost profit is zero then the scrap profit is not and vice versia 
                // !!!!!! check if this condition is valid or i must hard code two if conditions !!!!!!!!
                if (simulationCase.LostProfit != 0)
                    simulationsystem.PerformanceMeasures.DaysWithMoreDemand++;
                else
                    simulationsystem.PerformanceMeasures.DaysWithUnsoldPapers++;

                simulationsystem.SimulationTable.Add(simulationCase);

            }

        }

    }
}
