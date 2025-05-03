using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quiz.Repositories
{
    public class QuizRepository
    {
        private readonly string _filePath;
        private readonly string _password;
        private readonly byte[] _salt = Encoding.UTF8.GetBytes("quiz_salt1234");

        public QuizRepository(string filePath, string password)
        {
            _filePath = filePath;
            _password = password;
        }

        public List<Quiz.Models.Quiz> LoadAll()
        {
            if (!File.Exists(_filePath))
                return new List<Quiz.Models.Quiz>();

            try
            {
                byte[] encryptedData = File.ReadAllBytes(_filePath);
                string json = Decrypt(encryptedData);
                return JsonSerializer.Deserialize<List<Quiz.Models.Quiz>>(json) ?? new List<Quiz.Models.Quiz>();
            }
            catch
            {
                return new List<Quiz.Models.Quiz>();
            }
        }

        public void SaveAll(List<Quiz.Models.Quiz> quizzes)
        {
            string json = JsonSerializer.Serialize(quizzes, new JsonSerializerOptions { WriteIndented = true });
            byte[] encryptedData = Encrypt(json);
            var directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllBytes(_filePath, encryptedData);
        }

        public Quiz.Models.Quiz? GetByTitle(string title)
        {
            var quizzes = LoadAll();
            return quizzes.FirstOrDefault(q => q.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public void AddOrUpdate(Quiz.Models.Quiz quiz)
        {
            var quizzes = LoadAll();
            var existing = quizzes.FirstOrDefault(q => q.Title.Equals(quiz.Title, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                quizzes.Remove(existing);
            }
            quizzes.Add(quiz);
            SaveAll(quizzes);
        }

        private byte[] Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(_password, _salt, 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using var ms = new MemoryStream();
            using var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using var writer = new StreamWriter(cryptoStream);
            writer.Write(plainText);
            writer.Close();
            return ms.ToArray();
        }

        private string Decrypt(byte[] encryptedData)
        {
            using Aes aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(_password, _salt, 10000);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using var ms = new MemoryStream(encryptedData);
            using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }
    }
}
