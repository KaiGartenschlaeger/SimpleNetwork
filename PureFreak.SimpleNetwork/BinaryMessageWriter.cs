using System;
using System.IO;
using System.Text;

namespace PureFreak.SimpleNetwork
{
    /// <summary>
    /// Dient zum erstellen einer Binärnachricht.
    /// </summary>
    /// <example>
    /// Dieses Beispiel zeigt die Verwendung des BinaryMessageWriter.
    /// 
    /// <code>
    /// using (BinaryMessageWriter writer = new BinaryMessageWriter(server.Encoding))
    /// {
    ///     writer.WriteByte(1);
    ///     writer.WriteString("Server will be close in 5 Minutes!");
    ///     
    ///     server.Send(0, writer.GetBuffer());
    /// }
    /// </code>
    /// </example>
    public class BinaryMessageWriter : IDisposable
    {
        MemoryStream memStream;
        BinaryWriter writer;

        /// <summary>
        /// Dient zum erstellen einer Binärnachricht.
        /// </summary>
        public BinaryMessageWriter(Encoding encoding)
        {
            this.memStream = new MemoryStream();
            this.writer = new BinaryWriter(memStream, encoding);
        }

        /// <summary>
        /// Liefert die bisherige Länge in Bytes, die sich im Puffer befinden.
        /// </summary>
        public long BufferLength
        {
            get { return memStream.Length; }
        }

        /// <summary>
        /// Liefert die geschriebenen Daten aus dem Puffer.
        /// </summary>
        public byte[] GetBuffer()
        {
            return memStream.ToArray();
        }

        /// <summary>
        /// Schreibt einen bool in die Nachricht.
        /// </summary>
        public void WriteBool(bool value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen byte in die Nachricht.
        /// </summary>
        public void WriteByte(byte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt die angegebene Menge an Bytes in die Nachricht.
        /// </summary>
        public void WriteBytes(byte[] bytes)
        {
            writer.Write(bytes);
        }

        /// <summary>
        /// Schreibt einen sbyte in die Nachricht.
        /// </summary>
        public void WriteSByte(sbyte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen char in die Nachricht.
        /// </summary>
        public void WriteChar(char value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt die angegebene Menge an char in die Nachricht.
        /// </summary>
        public void WriteChars(char[] buffer)
        {
            writer.Write(buffer);
        }

        /// <summary>
        /// Schreibt einen short in die Nachricht.
        /// </summary>
        public void WriteShort(short value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen ushort in die Nachricht.
        /// </summary>
        public void WriteUShort(ushort value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen int in die Nachricht.
        /// </summary>
        public void WriteInt(int value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen uint in die Nachricht.
        /// </summary>
        public void WriteUInt(uint value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen long in die Nachricht.
        /// </summary>
        public void WriteLong(long value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen ulong in die Nachricht.
        /// </summary>
        public void WriteULong(ulong value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen double in die Nachricht.
        /// </summary>
        public void WriteDouble(double value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen float in die Nachricht.
        /// </summary>
        public void WriteFloat(float value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Schreibt einen string in die Nachricht.
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(string value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Verwendete Ressourcen freigeben.
        /// </summary>
        public void Dispose()
        {
            writer.Close();
        }
    }
}