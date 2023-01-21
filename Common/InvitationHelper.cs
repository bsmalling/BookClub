using System;
using System.Text;

namespace BookClub.Common
{

	public static class InvitationHelper
	{

		// Possibilities = 32^8 = 1.1*10^12
		private const int CODE_LENGTH = 8;

		// Removed 0, O, 1, l, and I because they can look ambiguous
		private const string CHARACTERS = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

		// Example code: "Lk3h-Bn92"
		public static string GenerateCode()
		{
			Random rand = new Random();
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < CODE_LENGTH; i++)
			{
                if (i == CODE_LENGTH / 2) builder.Append('-');
                builder.Append(CHARACTERS[rand.Next() % CHARACTERS.Length]);
			}
			return builder.ToString();
		}

		public static bool Matches(string code, string input)
		{
			return String.Equals(code, input, StringComparison.InvariantCultureIgnoreCase);
		}

    }

}
