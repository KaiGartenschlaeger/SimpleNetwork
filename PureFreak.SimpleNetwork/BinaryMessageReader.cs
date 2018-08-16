using System;
using System.IO;

namespace PureFreak.SimpleNetwork
{
    /// <summary>
    /// Dient zum auslesen von Binärnachrichten.
    /// </summary>
    /// <example>
    /// Dieses Beispiel demonstriert die Verwendung des BinaryMessageReader.
    /// 
    /// <code>
    /// using (BinaryMessageReader reader = new BinaryMessageReader(message.Buffer))
    /// {
    ///     int value1 = reader.ReadByte();
    ///     string value2 = reader.ReadString();
    /// }
    /// </code> 
    /// </example>
    public class BinaryMessageReader : IDisposable
    {
        private BinaryReader reader;

        /// <summary>
        /// Dient zum auslesen von Binärnachrichten.
        /// </summary>
        /// <param name="buffer">Der Datenpuffer der Nachricht.</param>
        public BinaryMessageReader(byte[] buffer)
        {
            reader = new BinaryReader(new MemoryStream(buffer));
        }

        /// <summary>
        /// Liefert die verbleibende Anzahl in Bytes, die sich noch im Puffer befinden.
        /// </summary>
        /// <value>Ein long der die Anzahl der verbleibenden Daten in Bytes angibt.</value>
        public long AvaibleBytes
        {
            get { return reader.BaseStream.Length - reader.BaseStream.Position; }
        }

        /// <summary>
        /// Liest einen bool aus der Nachricht.
        /// </summary>
        /// <returns><c>bool</c>, oder <c>false</c> falls keine weiteren Daten vorhanden sind.</returns>
        public bool ReadBool()
        {
            try
            {
                return reader.ReadBoolean();
            }
            catch (EndOfStreamException)
            {
                return false;
            }
        }

        /// <summary>
        /// Liest einen byte aus der Nachricht.
        /// </summary>
        /// <returns>byte, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public byte ReadByte()
        {
            try
            {
                return reader.ReadByte();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest die angegebene Menge an byte aus der Nachricht.
        /// </summary>
        /// <returns>bytes, oder ein leeres Array falls keine weiteren Daten vorhanden sind.</returns>
        public byte[] ReadBytes(int count)
        {
            try
            {
                return reader.ReadBytes(count);
            }
            catch (EndOfStreamException)
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// Liest einen sbyte aus der Nachricht.
        /// </summary>
        /// <returns>sbyte, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public sbyte ReadSByte()
        {
            try
            {
                return reader.ReadSByte();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen char aus der Nachricht.
        /// </summary>
        /// <returns>char, oder '\0' falls keine weiteren Daten vorhanden sind.</returns>
        public char ReadChar()
        {
            try
            {
                return reader.ReadChar();
            }
            catch (EndOfStreamException)
            {
                return '\0';
            }
        }

        /// <summary>
        /// Liest die angegebene Menge an char aus der Nachricht.
        /// </summary>
        /// <returns>chars, oder ein leeres Array falls keine weiteren Daten vorhanden sind.</returns>
        public char[] ReadChars(int count)
        {
            try
            {
                return reader.ReadChars(count);
            }
            catch (EndOfStreamException)
            {
                return new char[0];
            }
        }

        /// <summary>
        /// Liest einen short aus der Nachricht.
        /// </summary>
        /// <returns>short, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public short ReadShort()
        {
            try
            {
                return reader.ReadInt16();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen ushort aus der Nachricht.
        /// </summary>
        /// <returns>ushort, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public ushort ReadUShort()
        {
            try
            {
                return reader.ReadUInt16();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen int aus der Nachricht.
        /// </summary>
        /// <returns>int, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public int ReadInt()
        {
            try
            {
                return reader.ReadInt32();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen uint aus der Nachricht.
        /// </summary>
        /// <returns>uint, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public uint ReadUInt()
        {
            try
            {
                return reader.ReadUInt32();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen long aus der Nachricht.
        /// </summary>
        /// <returns>long, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public long ReadLong()
        {
            try
            {
                return reader.ReadInt64();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen ulong aus der Nachricht.
        /// </summary>
        /// <returns>ulong, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public ulong ReadULong()
        {
            try
            {
                return reader.ReadUInt64();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen double aus der Nachricht.
        /// </summary>
        /// <returns>double, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public double ReadDouble()
        {
            try
            {
                return reader.ReadDouble();
            }
            catch (EndOfStreamException)
            {
                return 0.0d;
            }
        }

        /// <summary>
        /// Liest einen float aus der Nachricht.
        /// </summary>
        /// <returns>float, oder 0 falls keine weiteren Daten vorhanden sind.</returns>
        public float ReadFloat()
        {
            try
            {
                return reader.ReadSingle();
            }
            catch (EndOfStreamException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Liest einen string aus der Nachricht.
        /// </summary>
        /// <returns>string, oder ein leerer string falls keine weiteren Daten vorhanden sind.</returns>
        public string ReadString()
        {
            try
            {
                return reader.ReadString();
            }
            catch (EndOfStreamException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Verwendete Ressourcen freigeben.
        /// </summary>
        public void Dispose()
        {
            reader.Close();
        }
    }
}