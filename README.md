# 💼 Md Julfiker Ali Jewel – Personal Portfolio Website

Welcome to my personal portfolio website repository! This site showcases my professional profile, academic journey, research, and projects in the field of Computer Science, Cybersecurity, and Artificial Intelligence.

🔗 [Live Website](https://mdjulfikeralijewel.com)  
📫 [Contact Me](mailto:jewelsheikh2013@gmail.com)

---

## 📌 About

This portfolio website was developed using **ASP.NET Core MVC** with **Razor views**, deployed via **Render**, and follows modern responsive design principles. It highlights:

- 👨‍💻 Software Development & Research Projects
- 🛡️ Cybersecurity R&D and AI-based IDS/IPS
- 📚 Academic Background (M.Sc. in Computer Science, WVSU)
- 🧠 Publications & Conference Presentations
- 📝 Resume & Skills
- 🌐 Professional Memberships (IEEE, ACM, etc.)
- 📷 Research and mentorship experiences

---

## 🧰 Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 8)
- **Language:** C#
- **View Engine:** Razor Pages
- **Styling:** HTML, CSS, Bootstrap, custom JavaScript
- **Hosting:** [Render.com](https://render.com)
- **Domain:** [Namecheap](https://www.namecheap.com/)

---

## 🚀 Features

- Dynamic routing for projects, blog posts, and publications
- Interactive CV/resume page
- Responsive design (mobile-first)
- Admin update-ready (for new publications or work updates)
- Secure deployment via HTTPS and environment configs

---

## 📂 Structure

/Portfolio
│
├── Controllers/
│ └── HomeController.cs
│
├── Views/
│ ├── Shared/
│ ├── Home/
│ └── Layout.cshtml
│
├── Models/
│ └── Project.cs, Research.cs
│
├── wwwroot/
│ ├── css/
│ ├── js/
│ └── assets/
│
├── appsettings.json
└── Program.cs / Startup.cs


---

## 📸 Screenshots

> Add screenshots here showing the Home Page, Projects section, Publications page, and Contact form.

---

## 🧪 Local Development

To run locally:

```bash
git clone https://github.com/Jewel777/MdJulfiker_site.git
cd MdJulfiker_site
dotnet restore
dotnet run --project Julfiker_Portfolio
```

Then open your browser and navigate to https://localhost:5001

Set `Email__Password`, `Admin__AnalyticsKey`, and `Analytics__IpHashSalt` as environment variables in production. Use long, randomly generated values for the two analytics secrets.

✍️ Author
Md Julfiker Ali Jewel
Graduate Researcher, Software Programmer, Cybersecurity & AI Enthusiast
📍 West Virginia, USA
📧 jewelsheikh2013@gmail.com

🌟 Contributing
This is a personal project, but suggestions and improvements are welcome. Feel free to open issues or pull requests.

