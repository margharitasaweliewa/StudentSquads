using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StudentSquads.Models;

namespace StudentSquads.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //public Person Person { get; set; }
        //public Guid? PersonId { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<FeePayment> FeePayments { get; set; }
        public DbSet<Squad> Squads { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<MainPosition> MainPositions { get; set; }
        public DbSet<HeadsOfStudentSquads> HeadsOfStudentSquads { get; set; }
        public DbSet<RegionalHeadquarter> RegionalHeadquarters { get; set; }
        public DbSet<UniversityHeadquarter> UniversityHeadquarters { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<WorkProject> WorkProjects { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<EventLevel> EventLevels { get; set; }
        public DbSet<Raiting> Raitings { get; set; }
        public DbSet<RaitingEvent> RaitingEvents { get; set; }
        public DbSet<RaitingSection> RaitingSections { get; set; }
        public DbSet<RaitingEventInfo> RaitingEventInfos { get; set; }
        public DbSet<RaitingEventInfoFile> RaitingEventInfoFiles { get; set; }
        public DbSet<RaitingPlace> RaitingPlaces { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<ProfessionSchool> ProfessionSchools { get; set; }
        public DbSet<ScholAttending> ScholAttendings { get; set; }
        public DbSet<WorkChangeType> WorkChangeTypes { get; set; }
        public DbSet<SquadWork> SquadWorks { get; set; }
        public DbSet<RaitingSectionLevel> RaitingSectionLevels { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}