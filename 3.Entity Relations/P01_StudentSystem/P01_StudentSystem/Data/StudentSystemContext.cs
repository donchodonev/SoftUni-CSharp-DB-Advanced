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

        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }

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
            //Resource
            /////////

            //Url

            modelBuilder.Entity<Resource>()
                .Property(x => x.Url)
                .IsUnicode(false);

            /////////
            //Homework
            /////////
         
            //Content
            modelBuilder.Entity<Homework>()
                .Property(x => x.Content)
                .IsUnicode(false);


            /////////
            //StudentCourse
            /////////

            modelBuilder.Entity<StudentCourse>()
                .HasKey(x => new { x.CourseId, x.StudentId });
        }
    }
}
