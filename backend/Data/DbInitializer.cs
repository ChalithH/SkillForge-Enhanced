using SkillForge.Api.Models;

namespace SkillForge.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if we already have skills
            if (context.Skills.Any())
            {
                return; // DB has been seeded
            }

            var skills = new Skill[]
            {
                // Programming & Development
                new() { Name = "Python", Category = "Programming", Description = "General-purpose programming language known for simplicity and versatility" },
                new() { Name = "JavaScript", Category = "Programming", Description = "Essential language for web development and increasingly used for backend" },
                new() { Name = "Java", Category = "Programming", Description = "Popular language for enterprise applications and Android development" },
                new() { Name = "C#", Category = "Programming", Description = "Microsoft's language for .NET development and Unity game development" },
                new() { Name = "React", Category = "Programming", Description = "Popular JavaScript library for building user interfaces" },
                new() { Name = "Angular", Category = "Programming", Description = "Full-featured framework for building web applications" },
                new() { Name = "Node.js", Category = "Programming", Description = "JavaScript runtime for server-side development" },
                new() { Name = "SQL", Category = "Programming", Description = "Language for managing and querying relational databases" },
                new() { Name = "Git", Category = "Programming", Description = "Version control system essential for collaborative development" },
                new() { Name = "Docker", Category = "Programming", Description = "Platform for containerizing applications" },

                // Design & Creative
                new() { Name = "UI/UX Design", Category = "Design", Description = "Creating user interfaces and experiences for digital products" },
                new() { Name = "Graphic Design", Category = "Design", Description = "Visual communication through typography, imagery, and layout" },
                new() { Name = "Adobe Photoshop", Category = "Design", Description = "Industry-standard software for image editing and manipulation" },
                new() { Name = "Figma", Category = "Design", Description = "Collaborative interface design tool" },
                new() { Name = "Video Editing", Category = "Design", Description = "Creating and editing video content" },

                // Business & Marketing
                new() { Name = "Digital Marketing", Category = "Business", Description = "Promoting products or services through digital channels" },
                new() { Name = "SEO", Category = "Business", Description = "Optimizing content to rank higher in search engines" },
                new() { Name = "Content Writing", Category = "Business", Description = "Creating written content for various purposes" },
                new() { Name = "Project Management", Category = "Business", Description = "Planning and executing projects efficiently" },
                new() { Name = "Data Analysis", Category = "Business", Description = "Interpreting data to make informed business decisions" },

                // Languages
                new() { Name = "English", Category = "Languages", Description = "Global language for business and communication" },
                new() { Name = "Spanish", Category = "Languages", Description = "Second most spoken language globally" },
                new() { Name = "French", Category = "Languages", Description = "International language of diplomacy and culture" },

                // Music & Arts
                new() { Name = "Guitar", Category = "Music", Description = "Popular string instrument for various music genres" },
                new() { Name = "Piano", Category = "Music", Description = "Fundamental keyboard instrument for music theory and performance" },
                new() { Name = "Photography", Category = "Arts", Description = "Capturing images with artistic and technical skill" },

                // Technology & Tools
                new() { Name = "Excel", Category = "Technology", Description = "Spreadsheet software for data analysis and organization" },
                new() { Name = "Machine Learning", Category = "Technology", Description = "Building systems that learn from data" },
                new() { Name = "Cloud Computing", Category = "Technology", Description = "Using remote servers for computing services" },
                new() { Name = "Cybersecurity", Category = "Technology", Description = "Protecting systems and data from digital attacks" }
            };

            context.Skills.AddRange(skills);
            context.SaveChanges();
        }
    }
}