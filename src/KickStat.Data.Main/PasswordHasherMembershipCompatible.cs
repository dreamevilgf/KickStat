using System.Security.Cryptography;
using KickStat.Data.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace KickStat.Data;

public class PasswordHasherMembershipCompatible : PasswordHasher<KickStatUser>
{
    // Переопределяем это, чтобы верифицировать старых пользователей
    /// <inheritdoc />
    [Obsolete("Obsolete")]
    public override PasswordVerificationResult VerifyHashedPassword(KickStatUser user, string hashedPassword, string providedPassword)
    {
        string[] passwordProperties = hashedPassword.Split('|');
        if (passwordProperties.Length != 3)
        {
            return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
        //else
        //{
        //    string passwordHash = passwordProperties[0];
        //    int passwordformat = 1;
        //    string salt = passwordProperties[2];
        //    if (string.Equals(EncryptPassword(providedPassword, passwordformat, salt), passwordHash, StringComparison.CurrentCultureIgnoreCase))
        //    {
        //        return PasswordVerificationResult.SuccessRehashNeeded;
        //    }
        //    else
        //    {
        //        return PasswordVerificationResult.Failed;
        //    }
        //}
        else
        {
            const int pbkdf2Count = 1000;
            const int pbkdf2SubkeyLength = 256 / 8;
            const int saltSize = 128 / 8;
            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) password hash.

            if (hashedPasswordBytes.Length != (1 + saltSize + pbkdf2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
            {
                // Wrong length or version header.
                return PasswordVerificationResult.SuccessRehashNeeded;
            }

            byte[] salt = new byte[saltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, saltSize);
            byte[] storedSubkey = new byte[pbkdf2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + saltSize, storedSubkey, 0, pbkdf2SubkeyLength);

            byte[] generatedSubkey;
            using var deriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, pbkdf2Count);
            generatedSubkey = deriveBytes.GetBytes(pbkdf2SubkeyLength);

            return storedSubkey.SequenceEqual(generatedSubkey) ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Failed;
        }
    }

    //This is copied from the existing SQL providers and is provided only for back-compat.
    //private string EncryptPassword(string pass, int passwordFormat, string salt)
    //{
    //    if (passwordFormat == 0) // MembershipPasswordFormat.Clear
    //        return pass;

    //    byte[] bIn = Encoding.Unicode.GetBytes(pass);
    //    byte[] bSalt = Convert.FromBase64String(salt);
    //    byte[] bRet = null;

    //    if (passwordFormat == 1)
    //    {
    //        // MembershipPasswordFormat.Hashed 
    //        HashAlgorithm hm = HashAlgorithm.Create("SHA1");
    //        if (hm is KeyedHashAlgorithm kha)
    //        {
    //            if (kha.Key.Length == bSalt.Length)
    //            {
    //                kha.Key = bSalt;
    //            }
    //            else if (kha.Key.Length < bSalt.Length)
    //            {
    //                byte[] bKey = new byte[kha.Key.Length];
    //                Buffer.BlockCopy(bSalt, 0, bKey, 0, bKey.Length);
    //                kha.Key = bKey;
    //            }
    //            else
    //            {
    //                byte[] bKey = new byte[kha.Key.Length];
    //                for (int iter = 0; iter < bKey.Length;)
    //                {
    //                    int len = Math.Min(bSalt.Length, bKey.Length - iter);
    //                    Buffer.BlockCopy(bSalt, 0, bKey, iter, len);
    //                    iter += len;
    //                }
    //                kha.Key = bKey;
    //            }
    //            bRet = kha.ComputeHash(bIn);
    //        }
    //        else
    //        {
    //            byte[] bAll = new byte[bSalt.Length + bIn.Length];
    //            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
    //            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
    //            bRet = hm.ComputeHash(bAll);
    //        }
    //    }

    //    return Convert.ToBase64String(bRet);
    //}
}