using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace ImageShareHW.Data
{
    public class ImageRepository
    {
        private string _connectionString;

        public ImageRepository(string cs)
        {
            _connectionString = cs;
        }

        public int AddImage(Images image)
        {
           using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO IMAGES (Password,ImagePath,Views) VALUES (@password,@image,0) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@image", image.Img);
            connection.Open();
            cmd.ExecuteNonQuery();
            return (int)(decimal)cmd.ExecuteScalar();
        }

        public Images GetImageById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM IMAGES WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

           return new Images
            {
                Id = (int)reader["Id"],
                Img = (string)reader["ImagePath"],
                Password = (string)reader["Password"],
                Views = (int)reader["Views"]

            };
           
        }

        public void UpdateImageViewCount(int id, int views)
        {
            using var connection  = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE IMAGES SET Views =@views WHERE Id = @id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@views", views);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

    }
}
