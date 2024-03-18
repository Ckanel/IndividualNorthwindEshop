using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IndividualNorthwindEshop.Services
{
    public class Ole78DecryptionService : IOle78DecryptionService
    {
        public byte[] DecryptData(byte[] encryptedData)
        {
            if (encryptedData == null)
            {
                return null;
            }

            if ( encryptedData.Length <= 78)
            {
                return encryptedData;
            }

            byte[] decryptedData = new byte[encryptedData.Length - 78];
            Array.Copy(encryptedData, 78, decryptedData, 0, decryptedData.Length);

            return decryptedData;
        }
       
    }
}
