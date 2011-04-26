#region License

// // Copyright 2010 John Rummell
// // 
// // This file is part of SteamAchievements.
// // 
// //     SteamAchievements is free software: you can redistribute it and/or modify
// //     it under the terms of the GNU General Public License as published by
// //     the Free Software Foundation, either version 3 of the License, or
// //     (at your option) any later version.
// // 
// //     SteamAchievements is distributed in the hope that it will be useful,
// //     but WITHOUT ANY WARRANTY; without even the implied warranty of
// //     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// //     GNU General Public License for more details.
// // 
// //     You should have received a copy of the GNU General Public License
// //     along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SteamAchievements.Data
{
    internal class DebugTextWriter : TextWriter
    {
        /// <summary>
        /// When overridden in a derived class, returns the <see cref="T:System.Text.Encoding"/> in which the output is written.
        /// </summary>
        /// <returns>The Encoding in which the output is written.</returns>
        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">Starting index in the buffer.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>. </exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer"/> parameter is null. </exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> or <paramref name="count"/> is negative. </exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        ///   
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(char[] buffer, int index, int count)
        {
            Debug.Write(new String(buffer, index, count));
        }

        /// <summary>
        /// Writes a string to the text stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed. </exception>
        ///   
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Write(string value)
        {
            Debug.Write(value);
        }
    }
}