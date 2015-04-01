using System;
using System.Collections.Generic;

namespace TennisTournament 
{
	public class Person 
	{
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public DateTime Dob { get; set; }
		public string Nationality { get; set; }
		public enum Gender { Mand, Kvinde }
		public Gender PersonGender { get; set; }

		public Person(string fn, string mn, string ln, DateTime d, string nat, Gender gen)
		{
			FirstName = fn; MiddleName = mn; LastName = ln; Dob = d; Nationality = nat; PersonGender = gen;
		}
	}

	public class TennisPlayer : Person 
	{
		public TennisPlayer(string fn, string mn, string ln, DateTime d, string nat, Gender gen)
		: base(fn, mn, ln, d, nat, gen)
		{
		}

		public int GetAge()
		{
			DateTime today = DateTime.Today;
			int age = today.Year - Dob.Year;
			if (today.Month < Dob.Month || (today.Month == Dob.Month && today.Day < Dob.Day)) age--;
			return age;
		}
	}

	public class Team
	{
		readonly TennisPlayer player1;
		readonly TennisPlayer player2;

		public Team(TennisPlayer p1)
		{
			player1 = p1;
		}

		public Team(TennisPlayer p1, TennisPlayer p2)
		{
			player1 = p1; player2 = p2;
		}

		public bool TeamSize()
		{
			return player2 != null ? true : false;
		}

		public string GetFirstName()
		{
			return TeamSize() ? String.Format("{0} & {1}", player1.FirstName, player2.FirstName) : String.Format("{0}", player1.FirstName);
		}

		public string GetFullName()
		{
			return TeamSize() ? String.Format("{0} {1} {2} & {3} {4} {5}", player1.FirstName, player1.MiddleName, player1.LastName, player2.FirstName, player2.MiddleName, player2.LastName) : String.Format("{0} {1} {2}", player1.FirstName, player1.MiddleName, player1.LastName);
		}

		public TennisPlayer ReturnP1()
		{
			return player1;
		}

		public TennisPlayer ReturnP2()
		{
			return player2 != null ? player2 : null;
		}
	}

	public class Referee : Person
	{
		public bool GamesMaster { get; set; }
		public DateTime LicenseAquired { get; set; }
		public DateTime LicenseRenewed { get; set; }

		public Referee(DateTime la, DateTime lr, string fn, string mn, string ln, DateTime d, string nat, Gender gen, bool gm = false)
		: base(fn, mn, ln, d, nat, gen)
		{
			GamesMaster = gm; LicenseAquired = la; LicenseRenewed = lr;
		}
	}

	public class Tournament 
	{
		public string Name { get; set; }
		public int Year { get; set; }
		public DateTime TStart { get; set; }
		public DateTime TEnd { get; set; }
		public int NoPlayers { get; set; }
		public List<Team> _playerList = new List<Team>();
		public List<Team> _roundWinners = new List<Team>();
		public List<Team> _tempPlayerList = new List<Team>();
		public List<Referee> _tournamentReferees = new List<Referee>();
		private int NoOfRounds;
		private Team tournamentWinner;

		public Tournament(string n, int y, DateTime from, DateTime to, int np = 8)
		{
			Name = n; Year = y; TStart = from; TEnd = to; NoPlayers = np;
			NoOfRounds = 1;
			int PlayersLeft = NoPlayers;
			while (PlayersLeft != 2)
			{
				PlayersLeft = PlayersLeft / 2;
				++NoOfRounds;
			}
		}

		public void PlayTournament(int noOfSets)
		{
			_roundWinners = _playerList;
			for(int j = 0; j < NoOfRounds; j++)
			{
				Console.WriteLine("ROUND {0}!", j+1);
				Console.Write("PLAYERS: ");
				_roundWinners.ForEach(i => Console.Write("{0}, ", i.GetFirstName()));
				Console.WriteLine();
				int NoOfMatches = _roundWinners.Count / 2;
				for (int i = 0; i < _roundWinners.Count; i = i+2)
				{
					Team winner_player;
					var match = new TennisMatch(_playerList[i], _playerList[i+1]);
					match.RunMatch(noOfSets, out winner_player);
					_tempPlayerList.Add(winner_player);
				}
				_roundWinners.Clear();
				for(int n = 0; n < _tempPlayerList.Count; n++)
				{
					_roundWinners.Add(_tempPlayerList[n]);
				}
				_tempPlayerList.Clear();
				if (_roundWinners.Count == 1)
				{
					tournamentWinner = _roundWinners[0];
					Console.WriteLine(ReturnWinner());
				}
			}
		}

		public void ReturnPlayerList()
		{
			_playerList.ForEach(i => Console.Write("{0}, ", i.GetFirstName()));
			Console.WriteLine();
		}

		public void ReturnRefereeList()
		{
			//_tournamentReferees.ForEach(i => Console.Write("{0}, ", i.FirstName));
			foreach (Referee referee in _tournamentReferees) 
			{
				Console.WriteLine("{0} - is Game Master? {1}", referee.FirstName, referee.GamesMaster);
			}
		}

		public void AddPlayer(Team player)
		{
			if(_playerList.Contains(player))
			{
				Console.WriteLine("Player is already competing at " + Name);
			}
			else 
			{
				_playerList.Add(player);
			}
		}

		public void RemovePlayer(Team player)
		{
			_playerList.Remove(player);
		}

		public void AddReferee(Referee referee)
		{
			if(_tournamentReferees.Contains(referee))
			{
				Console.WriteLine("Referee is already busy refereeing " + Name);
			}
			else
			{
				_tournamentReferees.Add(referee);
			}
			
		}

		public void RemoveReferee(Referee referee)
		{
			_tournamentReferees.Remove(referee);
		}

		public void AddGameMaster(Referee referee)
		{
			if(_tournamentReferees.Contains(referee)) 
			{
				referee.GamesMaster = true;
			}
			else
			{
				_tournamentReferees.Add(referee);
				referee.GamesMaster = true;
			}
		}

		public string ReturnWinner()
		{
			return string.Format("TOURNAMENT WINNER IS: {0}", tournamentWinner.GetFullName());
		}

		public override string ToString()
		{
			return "" + _playerList[0];
		}
	}

	public class TennisMatch 
	{
		readonly Team team1;
		readonly Team team2;
		static Random rand = new Random();

		public TennisMatch(Team team1, Team team2)
		{
			this.team1 = team1; this.team2 = team2;
		}

		/*public bool CheckGender()
		{
			return team1.PersonGender == team2.PersonGender ? true : false;
		}*/

		static void SingleSet(out int rand1, out int rand2)
		{
			rand1 = 0;
			rand2 = 0;

			while (rand1 != 6 & rand2 != 6)
			{
				rand1 = rand.Next(0, 7);
				rand2 = rand.Next(0, 7);
				while (rand1 == rand2)
				{
					rand1 = rand.Next(0, 7);
					rand2 = rand.Next(0, 7);
				}

			}
		}

		public string RunMatch(int noOfSets, out Team winner_player)
		{
			int rand1;
			int rand2;
			int cul_left = 0;
			int cul_right = 0;
			winner_player = team1;
			Console.WriteLine("\n{0} vs {1}", team1.GetFirstName(), team2.GetFirstName());
			for (int j = 0; j < noOfSets; j++)
			{
				SingleSet(out rand1, out rand2);
				if (rand1 == 6) { ++cul_left; } else { ++cul_right; }
				Console.WriteLine("" + rand1 + " - " + rand2);
			}
			if (cul_left > cul_right)
			{
				Console.WriteLine(team1.GetFirstName() + " wins! " + cul_left + " - " + cul_right);
				winner_player = team1;
			}
			else
			{
				Console.WriteLine(team2.GetFirstName() + " wins! " + cul_right + " - " + cul_left);
				winner_player = team2;
			}
			Console.WriteLine();
			return "";
		}
	}

	class Program 
	{
		static void Main(string[] args) 
		{
			var p1 = new TennisPlayer("Morten", "Fredsøe", "Mølgaard", new DateTime(1993, 07, 21), "Dansk", TennisPlayer.Gender.Mand);
			var p2 = new TennisPlayer("Simon", "van Deurs", "Brix", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var p3 = new TennisPlayer("Hans", "Peter", "Jensen", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var p4 = new TennisPlayer("Mikkel", "Olsen", "Lang", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var p5 = new TennisPlayer("Rune", "Gammel", "Høj", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var p6 = new TennisPlayer("Carsten", "Bruun", "Vestergaard", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var p7 = new TennisPlayer("Jakob", "J.", "Jakobsen", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var p8 = new TennisPlayer("Anders", "A.", "Andersen", new DateTime(1993, 10, 24), "Dansk", TennisPlayer.Gender.Mand);
			var team1 = new Team(p1);
			var team2 = new Team(p2);
			var team3 = new Team(p3);
			var team4 = new Team(p4);
			var team5 = new Team(p5);
			var team6 = new Team(p6);
			var team7 = new Team(p7);
			var team8 = new Team(p8);
			var team1d = new Team(p1, p2);
			var team2d = new Team(p3, p4);
			var team3d = new Team(p5, p6);
			var team4d = new Team(p7, p8);
			var ref1 = new Referee(new DateTime(2001, 08, 24), new DateTime(2014, 12, 24), "Kristoffer", "Mæng", "Nielsen", new DateTime(1991, 04, 01), "Dansk", Referee.Gender.Mand);
			var ref2 = new Referee(new DateTime(2003, 10, 02), new DateTime(2015, 01, 19), "Niclas", "Allentoft", "Jørgensen", new DateTime(1991, 08, 10), "Dansk", Referee.Gender.Mand);
			var tourn1 = new Tournament("Wimbledon", 2015, new DateTime(2015, 01, 15), new DateTime(2015, 02, 15), 4);
			var tourn2 = new Tournament("Wimbledon, I guess", 2015, new DateTime(2015, 01, 15), new DateTime(2015, 02, 15));
			tourn1.AddPlayer(team1d);
			tourn1.AddPlayer(team2d);
			tourn1.AddPlayer(team3d);
			tourn1.AddPlayer(team4d);
			tourn1.PlayTournament(3);
			/*tourn2.AddPlayer(team1);
			tourn2.AddPlayer(team2);
			tourn2.AddPlayer(team3);
			tourn2.AddPlayer(team4);
			tourn2.AddPlayer(team5);
			tourn2.AddPlayer(team6);
			tourn2.AddPlayer(team7);
			tourn2.AddPlayer(team8);
			tourn2.PlayTournament(3);*/
			/*tourn1.AddReferee(ref1);
			tourn1.AddReferee(ref2);
			tourn1.ReturnRefereeList();
			tourn1.AddGameMaster(ref1);
			tourn1.ReturnRefereeList();
			tourn1.RemoveReferee(ref1);
			tourn1.ReturnRefereeList();*/
		} 
	}

}