 <div align="center">

# Md Julfiker Ali Jewel
### Personal Portfolio · Research · Engineering

**A digital home for my work in Software Engineering, Cybersecurity, and Artificial Intelligence.**

[![Live Website](https://img.shields.io/badge/Live_Portfolio-Visit_Website-2563EB?style=for-the-badge&logo=googlechrome&logoColor=white)](https://mdjulfikeralijewel.com)
[![GitHub](https://img.shields.io/badge/GitHub-Jewel777-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Jewel777)
[![Email](https://img.shields.io/badge/Contact-Email_Me-0D9488?style=for-the-badge&logo=gmail&logoColor=white)](mailto:jewelsheikh2013@gmail.com)

<br>

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-ASP.NET_Core-239120?style=flat-square&logo=sharp&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-Responsive_UI-7952B3?style=flat-square&logo=bootstrap&logoColor=white)
![Render](https://img.shields.io/badge/Deployment-Render-46E3B7?style=flat-square&logo=render&logoColor=black)

</div>

---

## 01 / Overview

This repository contains the source code for my personal portfolio, built to present my professional experience, academic research, technical projects, and contributions to software engineering and cybersecurity.

The website brings together my work as a **Software Programmer, Computer Science Researcher, and AI & Cybersecurity Practitioner** within a single, accessible platform.

Designed with **ASP.NET Core MVC**, the portfolio emphasizes maintainability, responsive layouts, structured content, and straightforward navigation.

**Explore the portfolio:** [mdjulfikeralijewel.com](https://mdjulfikeralijewel.com)

---

## 02 / What You'll Find

| Area | Description |
|:--|:--|
| **Professional Experience** | Software development, enterprise systems, and technical experience |
| **Research & Publications** | Academic research, journal publications, and conference contributions |
| **Cybersecurity & AI** | Intrusion detection, machine learning, and security-focused research |
| **Featured Projects** | Selected software applications, research prototypes, and engineering work |
| **Education** | Academic background, qualifications, and achievements |
| **Technical Expertise** | Programming languages, frameworks, databases, and development tools |
| **Resume / CV** | Overview of professional qualifications and academic experience |
| **Professional Activities** | Technical reviewing, memberships, mentorship, and research involvement |
| **Contact** | Professional inquiries, collaborations, and networking |

---

## 03 / Technology Stack

The application is developed using the Microsoft .NET ecosystem with a conventional MVC architecture.

| Layer | Technology |
|:--|:--|
| **Backend** | ASP.NET Core MVC (.NET 8) |
| **Language** | C# |
| **View Engine** | Razor Views |
| **Frontend** | HTML5, CSS3, JavaScript |
| **UI Framework** | Bootstrap |
| **Hosting** | Render |
| **Domain Management** | Namecheap |
| **Version Control** | Git & GitHub |

### Architecture

```text
                       CLIENT / BROWSER
                              |
                              v
                     ASP.NET CORE MVC
                              |
              +---------------+---------------+
              |               |               |
              v               v               v
         CONTROLLERS        MODELS           VIEWS
              |               |               |
              +---------------+---------------+
                              |
                              v
                    RAZOR + BOOTSTRAP
                              |
                              v
                       RENDER HOSTING
                              |
                              v
                  mdjulfikeralijewel.com
```

---

## 04 / Core Capabilities

- **Responsive Interface:** Optimized layouts for desktop, tablet, and mobile browsing.
- **MVC Architecture:** Separation of application logic, data models, and presentation.
- **Structured Navigation:** Dedicated sections for projects, research, publications, and professional information.
- **Interactive Resume:** Accessible presentation of career and academic qualifications.
- **Extensible Content:** Organized structure for maintaining publications, projects, and portfolio updates.
- **Secure Configuration:** Environment-based management of application secrets.
- **HTTPS Deployment:** Public access through a custom domain with HTTPS.

---

## 05 / Project Structure

```text
MdJulfiker_site/
│
├── Julfiker_Portfolio/
│   │
│   ├── Controllers/
│   │   └── HomeController.cs
│   │
│   ├── Models/
│   │   ├── Project.cs
│   │   └── Research.cs
│   │
│   ├── Views/
│   │   ├── Home/
│   │   └── Shared/
│   │       └── _Layout.cshtml
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   └── assets/
│   │
│   ├── appsettings.json
│   └── Program.cs
│
└── README.md
```

> Representative MVC structure. Individual directories and files may vary with the current implementation.

---

## 06 / Getting Started

### Prerequisites

Ensure the following are installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- A code editor such as Visual Studio or Visual Studio Code

### Clone the Repository

```bash
git clone https://github.com/Jewel777/MdJulfiker_site.git
cd MdJulfiker_site
```

### Restore Dependencies

```bash
dotnet restore
```

### Run the Application

```bash
dotnet run --project Julfiker_Portfolio
```

Open the local HTTP or HTTPS address displayed in the terminal.

> The local port depends on the application's launch configuration.

---

## 07 / Environment Configuration

Certain application features require environment variables, especially when running in production.

| Variable | Purpose |
|:--|:--|
| `Email__Password` | Password or application credential for email functionality |
| `Admin__AnalyticsKey` | Secret key for protected analytics access |
| `Analytics__IpHashSalt` | Secret salt used for IP-related hashing |

### Security Considerations

- Never commit passwords, API keys, or secret values to the repository.
- Configure production secrets through Render's environment settings.
- Use long, cryptographically random values for analytics secrets.
- Avoid exposing sensitive configuration through client-side scripts.
- Rotate secrets if they are accidentally disclosed.

---

## 08 / Deployment

The website is hosted on **Render** and connected to a custom domain.

```text
GitHub Repository
       |
       v
 Application Build
       |
       v
 Render Deployment
       |
       v
 Custom Domain + HTTPS
       |
       v
 mdjulfikeralijewel.com
```

**Production URL:** https://mdjulfikeralijewel.com

Deployment configuration, build commands, and environment variables should be maintained according to the hosting environment.

---

## 09 / Website Preview

A visual preview of the portfolio's primary sections.

| Homepage | Projects |
|:--:|:--:|
| Screenshot coming soon | Screenshot coming soon |
| **Research & Publications** | **Contact** |
| Screenshot coming soon | Screenshot coming soon |

> Screenshots will be added to showcase the latest website interface.

---

## 10 / Future Improvements

Potential enhancements include:

- [ ] Expanded research and publication filtering
- [ ] Enhanced project case studies
- [ ] Improved accessibility and performance optimization
- [ ] Additional interactive portfolio components
- [ ] Simplified administrative content management

---

## 11 / Author

**Md Julfiker Ali Jewel**

Software Programmer | Computer Science Researcher | Cybersecurity & AI

M.S. in Computer Science, West Virginia State University

Based in West Virginia, United States.

**Connect**

- **Portfolio:** [mdjulfikeralijewel.com](https://mdjulfikeralijewel.com)
- **GitHub:** [github.com/Jewel777](https://github.com/Jewel777)
- **Email:** [jewelsheikh2013@gmail.com](mailto:jewelsheikh2013@gmail.com)

---

## 12 / Contributions

This repository represents a personal portfolio project.

Constructive feedback, improvement suggestions, and issue reports are welcome. For proposed changes, please open an issue or submit a pull request.

---

<div align="center">

**Built with ASP.NET Core MVC · Powered by .NET 8 · Deployed on Render**

[View Portfolio](https://mdjulfikeralijewel.com) · [Explore GitHub](https://github.com/Jewel777) · [Get in Touch](mailto:jewelsheikh2013@gmail.com)

<sub>© 2026 Md Julfiker Ali Jewel. All rights reserved.</sub>

</div>
