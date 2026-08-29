namespace VisitorManagment.Core.Generator
{
    public class Encoding64
    {
        public static string Base64Encoding64Method(string Data)
        {
            byte[] DataBytes = System.Text.Encoding.UTF8.GetBytes(Data);
            string sReturnValue = System.Convert.ToBase64String(DataBytes);
            return sReturnValue;
        }

        public static string Base64DecodingMethod(string Data)
        {
            byte [] DataBytes = System.Convert.FromBase64String(Data);
            string returnValue = System.Text.Encoding
                
                .UTF8.GetString(DataBytes);
            return returnValue;
        }
    }
}
