using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WARTLS
{
    class Tools
    {

        internal class DeflateTool
        {

            internal static string Decode(string base64encoded)
            {
                byte[] buff = Convert.FromBase64String(base64encoded);
                using (ZlibStream StreamZlib = new ZlibStream(new MemoryStream(buff), Ionic.Zlib.CompressionMode.Decompress))
                {
                    return new StreamReader(StreamZlib).ReadToEnd();
                }
            }
            internal static string Encode(string text)
            {
                byte[] buff = Encoding.UTF8.GetBytes(text);
                using (ZlibStream StreamZlib = new ZlibStream(new MemoryStream(buff), Ionic.Zlib.CompressionMode.Compress))
                {
                    return Convert.ToBase64String(ReadStream(StreamZlib));
                }
            }
            internal static byte[] ReadStream(Stream input)
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}
