# YourSoul Dating App

A full-fledged dating app built with .NET MAUI and MVVM architecture, using local SQLite database.

## Features

- **User Authentication**: Local login and registration system
- **User Discovery**: Swipe-based interface for discovering potential matches
- **Matching System**: Like/dislike functionality with mutual match detection
- **Chat System**: Messaging between matched users
- **Profile Management**: View and edit user profiles
- **Local Notifications**: Push notifications for new matches and messages

## Technologies Used

- **.NET MAUI**: Cross-platform UI framework
- **MVVM Architecture**: Clean separation of concerns
- **SQLite**: Local database for storing user data
- **CommunityToolkit.Mvvm**: MVVM implementation
- **Plugin.LocalNotification**: Local push notifications

## Getting Started

### Prerequisites

- Visual Studio 2022 or Visual Studio for Mac
- .NET 8.0 SDK
- MAUI workload installed

### Running the App

1. Clone the repository
2. Open the solution in Visual Studio
3. Build and run the application

### Sample Accounts

The app comes pre-loaded with sample users:
- Username: "john", Password: "password"
- Username: "sarah", Password: "password"
- Username: "mike", Password: "password"
- Username: "emily", Password: "password"
- Username: "alex", Password: "password"

## Project Structure

- **Models**: User, Match, Message, ChatConversation
- **ViewModels**: For each view with proper MVVM implementation
- **Views**: Login, Register, Discover, Matches, Chats, ChatDetail, Profile, UserProfile
- **Services**: Database, Authentication, Notification
- **Helpers**: Value converters for UI binding

## License

This project is licensed under the MIT License - see the LICENSE file for details.