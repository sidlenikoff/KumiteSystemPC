﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentTree
{
    public class Competitor
    {
        /// <Fouls>
        /// 1 - C,
        /// 2 - K,
        /// 3 - HC,
        /// 4 - H
        /// </Fouls>


        /// <Status>
        /// 0 - Active
        /// 1 - KIKEN
        /// 2 - SHIKAKU
        /// </Status>


        public delegate void CheckWinnerDelegate(bool isTimeUp=false);
        public event CheckWinnerDelegate Check_Winner;
        public int ID { get; set; } //TODO: Competitor ID
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Score { get; set; }
        List<int> AllScores { get; set; }
        public bool Senshu { get; set; }
        public int Fouls_C1 { get; set; }
        public int Fouls_C2 { get; set; }
        public int Status { get; set; }

        public bool IsBye { get; set; }

        public Competitor() { AllScores = new List<int>(); }

        public Competitor(Competitor competitor)
        {
            FirstName = competitor.FirstName;
            LastName = competitor.LastName;
            Score = competitor.Score;
            Fouls_C1 = competitor.Fouls_C1;
            Fouls_C2 = competitor.Fouls_C2;
            Status = competitor.Status;
            IsBye = competitor.IsBye;
            AllScores = competitor.AllScores;
        }

        public Competitor(bool isBye=false,int id=0,string FName="", string LName="", int score=0,int fc1=0,int fc2=0, int status =0)
        {
            IsBye = isBye;
            
            FirstName = FName;
            if (isBye) { FirstName = "BYE"; }
            LastName = LName;
            Score = score;
            Fouls_C1 = fc1;
            Fouls_C2 = fc2;
            Status = status;
            ID = id;
            AllScores = new List<int>();
        }

        public void AddPoints(int points)
        {
            Score += points;
            AllScores.Add(points);
           // Check_Winner?.Invoke();
        }

        public void SetFoulsC1(int fouls) { Fouls_C1 = fouls; Check_Winner?.Invoke(); }
        public void SetFoulsC2(int fouls) { Fouls_C2 = fouls; Check_Winner?.Invoke(); }

        public void ResetCompetitor()
        {
            AllScores = new List<int>();
            Fouls_C2 = 0;
            Fouls_C1 = 0;
            Score = 0;
        }
        public string GetName()
        {
            return $"{FirstName} {LastName}";
        }
        public string GetFoulsC1()
        {
            switch(Fouls_C1)
            {
                case 0:
                    return "";
                case 1:
                    return "C";
                case 2:
                    return "K";
                case 3:
                    return "HC";
                case 4:
                    return "H";
                default:
                    return "";
            }
        }
        public string GetFoulsC2()
        {
            switch (Fouls_C2)
            {
                case 0:
                    return "";
                case 1:
                    return "C";
                case 2:
                    return "K";
                case 3:
                    return "HC";
                case 4:
                    return "H";
                default:
                    return "";
            }
        }
        public override string ToString()
        {
            if (!IsBye) return $"{FirstName} {LastName}";
            else return "BYE";
        }
    }
}
