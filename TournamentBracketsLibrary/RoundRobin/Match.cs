﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoundRobin
{
    public class Match : TournamentsBracketsBase.IMatch
    {
        public int ID { get; set; }
        public TournamentsBracketsBase.ICompetitor AKA { get; set; }
        public TournamentsBracketsBase.ICompetitor AO { get; set; }
        //List<Competitor> Competitors { get; set; }
        public bool isFinished { get; set; }
        public event TournamentsBracketsBase.HaveWinnerHandler HaveWinner;

        public TournamentsBracketsBase.ICompetitor Winner { get; set; }
        public TournamentsBracketsBase.ICompetitor Looser { get; set; }

        public Match(Competitor _AKA, Competitor _AO, int id)
        {

            AKA = _AKA;
            AO = _AO;

            if (AKA != null && AKA.IsBye) 
            {
                Winner = new Competitor(AO as Competitor);
                (AO as Competitor).TotalScore += 3;
                isFinished = true; 
                Looser = new Competitor(AKA as Competitor); 

            }
            else if (AO != null && AO.IsBye) 
            {
                Winner = new Competitor(AKA as Competitor);
                (AKA as Competitor).TotalScore += 3;
                isFinished = true; 
                Looser = new Competitor(AO as Competitor); 
            }
            if (AKA != null) AKA.Check_Winner += CheckWinner;
            if (AO != null) AO.Check_Winner += CheckWinner;
            ID = id;
        }
        public bool IsAllCompetitors()
        {
            //TODO: All Conditions to check fullness of match
            if ((AKA.FirstName == null && AKA.LastName == null && AKA.ID == 0) ||
                (AO.FirstName == null && AO.LastName == null && AO.ID == 0) || AKA.IsBye || AO.IsBye) return false;
            else return true;
        }
        public void SetWinner(int winner, bool setLooser = true)
        {
            Competitor aka = AKA as Competitor;
            Competitor ao = AO as Competitor;
            if (isFinished && Winner.Equals(ao)) ao.TotalScore -= 3;
            else if (isFinished && Winner.Equals(aka)) aka.TotalScore -= 3;

            if (isFinished && Winner.IsBye) { ao.TotalScore--; aka.TotalScore--; }

            switch (winner)
            {
                case 1:
                    Winner = new Competitor(aka);
                    aka.TotalScore += 3;
                    if (setLooser) Looser = new Competitor(ao);
                    isFinished = true;
                    HaveWinner?.Invoke();
                    break;
                case 2:
                    Winner = new Competitor(ao);
                    ao.TotalScore += 3;
                    if (setLooser) Looser = new Competitor(aka);
                    isFinished = true;
                    HaveWinner?.Invoke();
                    break;
                case 0:
                    Winner = new Competitor(true);
                    aka.TotalScore++;
                    ao.TotalScore++;
                    if (setLooser) Looser = new Competitor(true);
                    isFinished = true;
                    HaveWinner?.Invoke();
                    break;
                default:
                    HaveWinner?.Invoke();
                    break;
            }
        }

        /*public void SetAKA(Competitor competitor)
        {
            AKA = competitor;
            AKA = competitor;
        }
        public void SetAO(Competitor competitor)
        {
            AO = competitor;
            AO = competitor;
        }*/



        public void CheckWinner(bool isTimeUP = false)
        {
            Competitor Aka = AKA as Competitor;
            Competitor Ao = AO as Competitor;

            if (Aka.Status == (int)Competitor.Statuses.Kiken || Aka.Status == (int)Competitor.Statuses.Shikaku) { SetWinner(2, false); }
            else if (Ao.Status == (int)Competitor.Statuses.Kiken || Ao.Status == (int)Competitor.Statuses.Shikaku) { SetWinner(1, false); }
            //
            else if (AKA != null && AKA.IsBye) { SetWinner(2); }
            else if (AO != null && AO.IsBye) { SetWinner(1); }
            //
            else if (Aka.Fouls_C1 >= (int)Competitor.Fouls.Hansoku /*|| Aka.Fouls_C2 >= 4*/) { SetWinner(2); }
            else if (Ao.Fouls_C1 >= (int)Competitor.Fouls.Hansoku /*|| Ao.Fouls_C2 >= 4*/) { SetWinner(1); }
            //
            else if (Aka.Score - 8 >= Ao.Score && Aka.Fouls_C1 < (int)Competitor.Fouls.Hansoku /*&& Aka.Fouls_C2 < 4*/) { SetWinner(1); }
            else if (Ao.Score - 8 >= Aka.Score && Ao.Fouls_C1 < (int)Competitor.Fouls.Hansoku /*&& Ao.Fouls_C2 < 4*/) { SetWinner(2); }
            //
            else if (isTimeUP && Aka.Score > Ao.Score && Aka.Fouls_C1 < (int)Competitor.Fouls.Hansoku /*&& Aka.Fouls_C2 < 4*/) { SetWinner(1); }
            else if (isTimeUP && Ao.Score > Aka.Score && Ao.Fouls_C1 < (int)Competitor.Fouls.Hansoku /*&& Ao.Fouls_C2 < 4*/) { SetWinner(2); }
            //
            else if (isTimeUP && Aka.Score == Ao.Score && Aka.Senshu) { SetWinner(1); }
            else if (isTimeUP && Aka.Score == Ao.Score && Ao.Senshu) { SetWinner(2); }
            //
            else if (isTimeUP)
            {
                int countIpAka = 0, countWazAka = 0;
                foreach (var sc in Aka.AllScores)
                {
                    if (sc == 3) countIpAka++;
                    else if (sc == 2) countWazAka++;
                }
                foreach (var sc in Ao.AllScores)
                {
                    if (sc == 3) countIpAka--;
                    else if (sc == 2) countWazAka--;
                }

                if (countIpAka > 0) SetWinner(1);
                else if (countIpAka < 0) SetWinner(2);
                else
                {
                    if (countWazAka > 0) SetWinner(1);
                    else if (countWazAka < 0) SetWinner(2);
                }

                if (!isFinished)
                    SetWinner(0);
            }
            //
            //TODO: All conditions to Set winner

            //Set tie (only for Round Robin)
            //SetWinner(0);

        }
        public void Reset()
        {
            AKA.ResetCompetitor();
            AO.ResetCompetitor();
        }
        public override string ToString()
        {
            if (AKA != null && AO != null && (AKA.IsBye || AO.IsBye)) { return $"/ - /"; }
            else return $"{AKA} - {AO}";
        }
    }
}
