using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {

        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Integrated Security=true;Database=StudentSystem");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /////////
            //STUDENT
            /////////

            //Name

            modelBuilder
                .Entity<Student>()
                .Property(x => x.StudentId)
                .IsUnicode();

            //PhoneNumber

            modelBuilder
                .Entity<Student>().Property(x => x.PhoneNumber)
                .IsFixedLength();


            /////////
            //Course
            /////////

            //Name

            modelBuilder.Entity<Course>()
                .Property(x => x.Name)
                .IsUnicode();

            /////////
            //Course
            /////////

            //Url

            modelBuilder.Entity<Resource>()
                .Property(x => x.Url)
                .IsUnicode(false);
        }
    }
}
