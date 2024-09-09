# Server Student Accommodation
I developed a server in ASP.NET Core for a web application that manages student housing in dormitories. The system provides complete functionality, from applying for dormitory spots to managing student documents and assigning them to rooms.

## Key Features
- **Authentication and Authorization**:
  - User authentication (students and administrators) with appropriate role and permission handling.
- **Document Management for Housing**:
  - Students can upload their required documents, and administrators can approve or reject them based on validity.
- **Automatic Room Allocation**:
  - Assigns rooms based on predefined criteria to ensure fair distribution.
- **Admin Dashboard**:
  - Interface for administrators to manage users, rooms, and student documents.
- **Notification and Announcement System**:
  - Students can view notifications and announcements posted by administrators.
  - Admins can send individual notifications to students and post announcements visible to all students.

## Technologies Used
- **Backend**: ASP.NET Core
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL Server
- **Deployment**: Docker container running the database

