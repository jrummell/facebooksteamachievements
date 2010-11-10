#region License

// Copyright 2010 John Rummell
// 
// This file is part of SteamAchievements.
// 
//     SteamAchievements is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     SteamAchievements is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Text;
using System.Web.Script.Serialization;
using NUnit.Framework;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class CanvasAuthenticationFixture
    {
        [Serializable]
        private class SignedRequest
        {
            public string algorithm { get; set; }
            public string issued_at { get; set; }
            public long user_id { get; set; }
            public string oauth_token { get; set; }
            public int expires { get; set; }
            public long profile_id { get; set; }
        }

        ///<summary>
        /// Decode Base64 encoded string with URL and Filename Safe Alphabet using UTF-8.
        /// http://www.vbforums.com/showthread.php?t=287324
        ///</summary>
        ///<param name="str">Base64 code</param>
        ///<returns>The decoded string.</returns>
        public static string Base64Decode(string base64)
        {
            base64 = base64.PadRight(base64.Length + (4 - base64.Length%4)%4, '=');

            UTF8Encoding encoder = new UTF8Encoding();
            Decoder utf8Decode = encoder.GetDecoder();

            byte[] todecode_byte = Convert.FromBase64String(base64);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        [Test]
        public void Authenticate()
        {
            // http://developers.facebook.com/docs/authentication/canvas
            string signedRequest =
                "vlXgu64BQGFSQrY0ZcJBZASMvYvTHu9GQ0YM9rjPSso.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsIjAiOiJwYXlsb2FkIn0";

            string[] signedRequestParts = signedRequest.Split('.');

            string sig = Base64Decode(signedRequestParts[0]);
            Console.WriteLine("sig: " + sig);

            string payload = Base64Decode(signedRequestParts[1]);
            Console.WriteLine("payload: " + payload);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            SignedRequest data = serializer.Deserialize<SignedRequest>(payload);

            Assert.That(data, Is.Not.Null);
        }
    }
}