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
		public List<TennisPlayer> _playerList = new List<TennisPlayer>();
		public List<TennisPlayer> _roundWinners = new List<TennisPlayer>();
		public List<TennisPlayer> _tempPlayerList = new List<TennisPlayer>();
		public List<Referee> _tournamentReferees = new List<Referee>();
		private int NoOfRounds;
		private int count;
		private TennisPlayer tournamentWinner;

		public Tournament(string n, int y, DateTime from, DateTime to, int np = 8, params TennisPlayer[] args)
		{
			Name = n; Year = y; TStart = from; TEnd = to; NoPlayers = np; count = args.Length;
			for (int i = 0; i < count; i++)
			{
				_playerList.Add(args[i]);
			}
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
				_roundWinners.ForEach(i => Console.Write("{0}, ", i.FirstName));
				Console.WriteLine();
				int NoOfMatches = _roundWinners.Count / 2;
				for (int i = 0; i < _roundWinners.Count; i = i+2)
				{
					TennisPlayer winner_player;
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
			_playerList.ForEach(i => Console.Write("{0}, ", i.FirstName));
			Console.WriteLine();
		}

		public void ReturnRefereeList()
		{
			_tournamentReferees.ForEach(i => Console.Write("{0}, ", i.FirstName));
		}

		public void AddPlayer(TennisPlayer player)
		{
			_playerList.Add(player);
		}

		public void RemovePlayer(TennisPlayer player)
		{
			_playerList.Remove(player);
		}

		public void AddReferee(Referee referee)
		{
			_tournamentReferees.Add(referee);
		}

		public void RemoveReferee(Referee referee)
		{
			_tournamentReferees.Remove(referee);
		}

		public string ReturnWinner()
		{
			return string.Format("TOURNAMENT WINNER IS: {0} {1} {2} at the age of {3}", tournamentWinner.FirstName, tournamentWinner.MiddleName, tournamentWinner.LastName, tournamentWinner.GetAge());
		}

		public override string ToString()
		{
			return "" + _playerList[0];
		}
	}

	public class TennisMatch 
	{
		readonly TennisPlayer player1;
		readonly TennisPlayer player2;
		static Random rand = new Random();

		public TennisMatch(TennisPlayer player1, TennisPlayer player2)
		{
			this.player1 = player1; this.player2 = player2;
		}

		public bool CheckGender()
		{
			return player1.PersonGender == player2.PersonGender ? true : false;
		}

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

		public string RunMatch(int noOfSets, out TennisPlayer winner_player)
		{
			int rand1;
			int rand2;
			int cul_left = 0;
			int cul_right = 0;
			winner_player = player1;
			Console.WriteLine("\n{0} vs {1}", player1.FirstName, player2.FirstName);
			for (int j = 0; j < noOfSets; j++)
			{
				SingleSet(out rand1, out rand2);
				if (rand1 == 6) { ++cul_left; } else { ++cul_right; }
				Console.WriteLine("" + rand1 + " - " + rand2);
			}
			if (cul_left > cul_right)
			{
				Console.WriteLine(player1.FirstName + " wins! " + cul_left + " - " + cul_right);
				winner_player = player1;
			}
			else
			{
				Console.WriteLine(player2.FirstName + " wins! " + cul_right + " - " + cul_left);
				winner_player = player2;
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
			var ref1 = new Referee(new DateTime(2001, 08, 24), new DateTime(2014, 12, 24), "Kristoffer", "Mæng", "Nielsen", new DateTime(1991, 04, 01), "Dansk", Referee.Gender.Mand);
			var ref2 = new Referee(new DateTime(2003, 10, 02), new DateTime(2015, 01, 19), "Niclas", "Allentoft", "Jørgensen", new DateTime(1991, 08, 10), "Dansk", Referee.Gender.Mand);
			var tourn1 = new Tournament("Wimbledon", 2015, new DateTime(2015, 01, 15), new DateTime(2015, 02, 15), 8, p1, p2, p3, p4, p5, p6, p7, p8);
			var tourn2 = new Tournament("Wimbledon, I guess", 2015, new DateTime(2015, 01, 15), new DateTime(2015, 02, 15));
			//tourn1.ReturnPlayerList();
			//tourn1.PlayTournament(3);
			/*tourn2.AddPlayer(p1);
			tourn2.AddPlayer(p2);
			tourn2.AddPlayer(p3);
			tourn2.AddPlayer(p4);
			tourn2.AddPlayer(p5);
			tourn2.AddPlayer(p6);
			tourn2.AddPlayer(p7);
			tourn2.AddPlayer(p8);
			tourn2.PlayTournament(3);*/
			/*tourn1.AddReferee(ref1);
			tourn1.AddReferee(ref2);
			tourn1.ReturnRefereeList();
			tourn1.RemoveReferee(ref1);
			tourn1.ReturnRefereeList();*/
		} 
	}

}