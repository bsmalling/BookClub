using System;
using System.Text;

namespace BookClub.Common
{

	public static class InvitationHelper
	{

		// Possibilities = 29^8 = 5.0*10^11 = 50,024,641,300
		private const int CODE_LENGTH = 8;

		// Removed 0, O, 1, l, I, 5, and S because they can look ambiguous
		private const string CHARACTERS = "2346789ABCDEFGHJKLMNPQRTUVWXYZ";

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
