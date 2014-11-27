using System;

namespace FaceWebApi
{
    /// <summary>
    /// Tratamiento de ficheros y cadenas en base64
    /// </summary>
    class ClsEncoder64
    {
        /// <summary>
        /// Códifica en base64 una cadena de texto
        /// </summary>
        /// <param name="toEncode">Cadena a convertir</param>
        /// <returns>Cadena convertida e base64</returns>
        public static string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes  = System.Text.Encoding.Unicode.GetBytes(toEncode);
            string returnValue      = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
        /// <summary>
        /// Descodifica una cadena en base64
        /// </summary>
        /// <param name="toDecode">Cadena en base64 para descodificar</param>
        /// <returns>Cadena descoficada</returns>
        public static string DecodeFrom64(string toDecode)
        {

            byte[] toDecodeAsBytes  = System.Convert.FromBase64String(toDecode);
            string returnValue      = System.Text.Encoding.Unicode.GetString(toDecodeAsBytes);

            return returnValue;

        }
        /// <summary>
        /// Codifica un fichero en base64
        /// </summary>
        /// <param name="toEncodeFile">Ruta del fichero a codificar</param>
        /// <returns>Cadena base64 resultante de codificar el fichero</returns>
        public string EncodeTo64File(string toEncodeFile)
        { 
            string toEncode;

            using (System.IO.FileStream fs = new System.IO.FileStream(toEncodeFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                byte[] filebytes = new byte[fs.Length];
        		fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
				toEncode = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
            }
		    
            return toEncode;
        }
        /// <summary>
        /// Crea un fichero descodificado.
        /// </summary>
        /// <param name="toDecodeFile">Ruta del nuevo fichero</param>
        /// <psaram name="toDecode">Cadena en base64 con el contenido del fichero</param>
        public void DecodeFromBase64File(string toDecodeFile, string toDecode)
        { 
            byte[] filebytes = Convert.FromBase64String(toDecode);

		    using (System.IO.FileStream fs = new System.IO.FileStream(toDecodeFile, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                fs.Write(filebytes, 0, filebytes.Length); 
				fs.Close(); 
            }
        }
    }
}
