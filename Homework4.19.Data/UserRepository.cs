using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
namespace Homework4._19.Data
{
    public class UserRepository
    {
        private string _connection { get; set; }
        public UserRepository(string connection)
        {
            _connection = connection;
        }
        public void AddUser(User user)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"INSERT Into Users(Name, Email, PasswordHash)
                                  VALUES(@name, @email, @passwordHash)";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", passwordHash);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddAd(Ad ad)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO Ads(PhoneNumber, Description, UserId, DatePosted)
                                  VALUES(@phoneNumber, @description, @userId, GETDATE())";
            command.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            command.Parameters.AddWithValue("@description", ad.Description);
            command.Parameters.AddWithValue("@userId", ad.UserId);
            connection.Open();
            command.ExecuteNonQuery();
        }
        private User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT * FROM Users WHERE Email = @email";
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User()
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["email"],
                Password = (string)reader["PasswordHash"],
            };
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            var isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isValid)
            {
                return null;
            }
            return user;
        }
        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"Select * From Ads a
                                  Join Users u
                                  ON a.UserId = u.Id";
            connection.Open();
            var reader = command.ExecuteReader();
            var ads = new List<Ad>();
            while (reader.Read())
            {
                ads.Add(new Ad()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    DatePosted = (DateTime)reader["DatePosted"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }
        public int GetUserId(string email)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT Id FROM Users
                                  WHERE Email = @email";
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            return (int)command.ExecuteScalar();
        }
        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"DELETE Ads
                                  WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Ad> GetAdsForUser(string email)
        {
            using var connection = new SqlConnection(_connection);
            using var command = connection.CreateCommand();
            command.CommandText = $@"Select * From Ads a
                                  Join Users u
                                  ON a.UserId = u.Id
                                  WHERE Email = @email";
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = command.ExecuteReader();
            var ads = new List<Ad>();
            while (reader.Read())
            {
                ads.Add(new Ad()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    DatePosted = (DateTime)reader["DatePosted"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;
        }
    }
}
