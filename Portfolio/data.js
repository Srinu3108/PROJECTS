// ========================================
// PORTFOLIO DATA - EASY TO UPDATE
// ========================================
// Update all your information here - no need to touch HTML/CSS!

const portfolioData = {
    // PERSONAL INFORMATION
    personal: {
        name: "Srinu",
        title: "Full Stack Developer & Problem Solver",
        description: "I build smart web applications that solve real-world problems using ASP.NET Core, Blazor, SQL Server, Java, and modern web technologies.",
        
        // CONTACT DETAILS (Update these with your actual info)
        email: "kanuparthicnu@gmail.com",
        phone: "+91-7981210743",
        location: "Chennai, India",
        
        // SOCIAL LINKS
        github: "https://github.com/Srinu3108",
        linkedin: "https://www.linkedin.com/in/srinivasulu-k-36b862270/",
        twitter: "https://twitter.com/yourhandle",
        
        // ABOUT ME
        aboutMe: "I'm a passionate full-stack developer with expertise in building scalable web applications. With a strong foundation in both frontend and backend technologies, I create solutions that are not only functional but also user-friendly and performant.",
        aboutMe2: "My approach focuses on understanding client needs, delivering quality code, and ensuring seamless user experiences. I'm committed to continuous learning and staying updated with the latest technology trends.",
        aboutMe3: "When I'm not coding, I enjoy problem-solving through competitive programming and contributing to open-source projects.",
    },

    // SERVICES/EXPERTISE
    services: [
        {
            icon: "fas fa-code",
            title: "Web Development",
            description: "Full-stack web applications using ASP.NET Core, Blazor, and modern JavaScript frameworks. Responsive, scalable, and optimized for performance."
        },
        {
            icon: "fas fa-database",
            title: "Database Design",
            description: "SQL Server database architecture, optimization, and management. Complex queries, stored procedures, and data modeling expertise."
        },
        {
            icon: "fas fa-mobile-alt",
            title: "Responsive Design",
            description: "Mobile-first responsive designs that work seamlessly across all devices and screen sizes."
        },
        {
            icon: "fas fa-cogs",
            title: "API Development",
            description: "RESTful and modern APIs with proper authentication, validation, and documentation for third-party integration."
        },
        {
            icon: "fas fa-bug",
            title: "Debugging & Optimization",
            description: "Code optimization, performance tuning, and fixing complex bugs to ensure production-ready applications."
        },
        {
            icon: "fas fa-graduation-cap",
            title: "Consultation",
            description: "Technology consulting, architecture guidance, and best practices for your projects and business needs."
        }
    ],

    // PROJECTS (Update with your projects)
    projects: [
        {
            name: "EnergyPulse",
            description: "Smart Energy Monitoring System with real-time alerts, comprehensive analytics dashboard, maintenance reports, and IoT device monitoring. Built for efficient energy management and cost optimization.",
            technologies: ["ASP.NET Core", "React", "SQL Server", "WebSockets"],
            github: "https://github.com/Srinu3108/PROJECTS",
            liveDemo: "https://github.com/Srinu3108/PROJECTS"
        },
        {
            name: "Farm Management System",
            description: "Comprehensive Farm Management and Crop Monitoring system with real-time crop health tracking, weather forecasting integration, yield predictions, and automated irrigation management.",
            technologies: ["Java", "Spring Boot", "MySQL", "Machine Learning"],
            github: "https://github.com/Srinu3108/PROJECTS",
            liveDemo: "https://github.com/Srinu3108/PROJECTS"
        }
    ],

    // EXPERIENCE
    experience: [
        {
            years: "2023 - Present",
            title: "Full Stack Developer",
            company: "Tech Solutions Inc.",
            highlights: [
                "Developed 5+ enterprise web applications using ASP.NET Core and Blazor",
                "Designed and optimized SQL Server databases handling 100k+ records",
                "Improved application performance by 40% through code optimization",
                "Led code reviews and mentored 2 junior developers"
            ]
        },
        {
            years: "2021 - 2023",
            title: "Junior Developer",
            company: "Digital Innovations Ltd.",
            highlights: [
                "Built responsive web applications using HTML, CSS, and JavaScript",
                "Collaborated with designers to implement UI/UX designs",
                "Fixed bugs and maintained existing codebases",
                "Participated in agile development practices"
            ]
        },
        {
            years: "2020 - 2021",
            title: "Intern Developer",
            company: "StartUp Hub",
            highlights: [
                "Assisted in web development projects",
                "Learned version control and collaborative development",
                "Contributed to open-source projects"
            ]
        }
    ],

    // EDUCATION & CERTIFICATIONS
    education: [
        {
            icon: "fas fa-graduation-cap",
            title: "Bachelor of Technology in Computer Science",
            institution: "Saveetha University - 2022",
            details: "CGPA: 8.9/10 | Specialized in Web Development and Database Management"
        },
        {
            icon: "fas fa-certificate",
            title: "Oracle Certified Associate: Java Programmer",
            institution: "Oracle - Java SE 17",
            details: "Certified in core Java programming, object-oriented concepts, and Java SE 17 fundamentals"
        },
        {
            icon: "fas fa-certificate",
            title: "Full Stack Development Bootcamp",
            institution: "CodePath - 2022",
            details: "Intensive program covering MERN stack and modern web development practices"
        }
    ],

    // SKILLS (organized by category)
    skills: {
        backend: ["ASP.NET Core", "C#", "Java", "Node.js", "Python"],
        frontend: ["HTML5", "CSS3", "JavaScript", "React", "Blazor"],
        databases: ["SQL Server", "MySQL", "MongoDB", "Firebase"],
        tools: ["Git/GitHub", "Docker", "Azure", "REST APIs", "WebSockets"]
    },

    // TESTIMONIALS
    testimonials: [
        {
            text: "Srinu delivered our project on time with exceptional quality. His attention to detail and problem-solving skills were impressive. Highly recommended!",
            author: "Raj Kumar",
            title: "CEO at TechStart",
            stars: 5
        },
        {
            text: "Outstanding developer with great communication. He understood our requirements perfectly and built exactly what we needed.",
            author: "Priya Sharma",
            title: "Product Manager at InnovateLabs",
            stars: 5
        },
        {
            text: "Professional, reliable, and highly skilled. Srinu went above and beyond to ensure our satisfaction. We're planning to work with him again.",
            author: "Amit Patel",
            title: "Founder at WebSolutions",
            stars: 5
        }
    ]
};

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = portfolioData;
}
