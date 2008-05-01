using System.Collections.Generic;
using NUnit.Framework;

namespace YTech.WebControls.ContentRotator
{
	[TestFixture]
	public class ServerContentRotator_Tester
	{
		[Test]
		public void ChooseKey()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 100);
			keys.Add("def", 0);

			Assert.AreEqual("abc", ServerContentRotator.ChooseKey(keys));
		}

		[Test]
		public void ChooseKey2()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 50);
			keys.Add("def", 50);

			int abcCount = 0, defCount = 0;

			for (int i = 0; i < 100000; i++)
			{
				string key = ServerContentRotator.ChooseKey(keys);

				if (key == "abc")
					abcCount++;
				else if (key == "def")
					defCount++;
			}

			//Since we're doing it 100,000 times, the distribution should be fairly even
			Assert.IsTrue(abcCount > 40000);
			Assert.IsTrue(defCount > 40000);
			Assert.AreEqual(100000, abcCount + defCount);
		}

		//Always different
		[Test]
		public void ChooseKey3()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 50);
			keys.Add("def", 50);

			for (int i = 0; i < 10; i++)
			{
				string key = ServerContentRotator.ChooseKey(keys, RotationModes.AlwaysDifferent, "abc");

				Assert.AreEqual("def", key);
			}
		}

		//Always same
		[Test]
		public void ChooseKey4()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 50);
			keys.Add("def", 50);

			for (int i = 0; i < 10; i++)
			{
				string key = ServerContentRotator.ChooseKey(keys, RotationModes.AlwaysSame, "abc");

				Assert.AreEqual("abc", key);
			}
		}

		//Random
		[Test]
		public void ChooseKey5()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 50);
			keys.Add("def", 50);

			int abcCount = 0, defCount = 0;

			for (int i = 0; i < 100000; i++)
			{
				string key = ServerContentRotator.ChooseKey(keys, RotationModes.Random, "abc");

				if (key == "abc")
					abcCount++;
				else if (key == "def")
					defCount++;
			}

			//Since we're doing it 100,000 times, the distribution should be fairly even
			Assert.IsTrue(abcCount > 40000);
			Assert.IsTrue(defCount > 40000);
			Assert.AreEqual(100000, abcCount + defCount);
		}

		/// <summary>
		///		Verify that if the key is always different, it can handle a NULL previous day
		/// </summary>
		[Test]
		public void ChooseKey6()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 50);
			keys.Add("def", 50);

			Assert.IsNotNull(ServerContentRotator.ChooseKey(keys, RotationModes.AlwaysDifferent, null));
		}

		/// <summary>
		///		Verify that if the key is always the same, it can handle a NULL previous day
		/// </summary>
		[Test]
		public void ChooseKey7()
		{
			Dictionary<string, int> keys = new Dictionary<string, int>();
			keys.Add("abc", 50);
			keys.Add("def", 50);

			Assert.IsNotNull(ServerContentRotator.ChooseKey(keys, RotationModes.AlwaysSame, null));
		}
	}
}
