# YourSoul Dating App

A full-fledged dating app built with .NET MAUI and MVVM architecture, designed for cross-platform deployment on iOS, Android, macOS, and Windows. YourSoul helps users find their perfect match through an intuitive, swipe-based interface and real-time messaging.

![YourSoul App](https://via.placeholder.com/800x400?text=YourSoul+Dating+App)

## Features

- **Secure User Authentication**: Registration and login with secure password hashing
- **User Discovery**: Swipe-based interface for discovering potential matches
- **Matching System**: Like/dislike functionality with mutual match detection
- **Real-time Chat**: Messaging between matched users
- **Comprehensive Profile Management**: View and edit detailed user profiles
- **Local Notifications**: Push notifications for new matches and messages
- **Cross-Platform**: Works on iOS, Android, macOS, and Windows

## Technologies Used

- **.NET MAUI**: Cross-platform UI framework for building native mobile and desktop apps
- **MVVM Architecture**: Clean separation of concerns with Model-View-ViewModel pattern
- **SQLite**: Local database for storing user data and messages
- **CommunityToolkit.Mvvm**: Modern MVVM implementation with source generators
- **Plugin.LocalNotification**: Cross-platform local push notifications
- **Secure Password Hashing**: PBKDF2 with HMACSHA256 for secure credential storage

## Getting Started

### Prerequisites

- Visual Studio 2022 (Windows) or Visual Studio for Mac 2022
- .NET 8.0 SDK or later
- MAUI workload installed
- For iOS/macOS development: Mac computer with Xcode 14 or later
- For Android development: Android SDK with API level 33 or later

### Running the App

1. Clone the repository
   ```
   git clone https://github.com/Mahesh199811/YourSoul.git
   ```

2. Open the solution in Visual Studio
   ```
   cd YourSoul
   start YourSoul.sln    # On Windows
   open YourSoul.sln     # On macOS
   ```

3. Select your target platform (Android, iOS, macOS, or Windows)

4. Build and run the application

### Sample Accounts

The app comes pre-loaded with sample users for testing:
- Username: "john", Password: "password" - 28-year-old male from New York
- Username: "sarah", Password: "password" - 26-year-old female from Boston
- Username: "mike", Password: "password" - 30-year-old male from San Francisco
- Username: "emily", Password: "password" - 25-year-old female from Chicago
- Username: "alex", Password: "password" - 29-year-old male from Los Angeles

## Project Structure

- **Models/**: Data models representing core entities
  - User, Match, Message, ChatConversation

- **ViewModels/**: MVVM implementation for each view
  - LoginViewModel, RegisterViewModel, DiscoverViewModel, etc.

- **Views/**: UI pages and components
  - Login, Register, Discover, Matches, Chats, ChatDetail, Profile, UserProfile

- **Services/**: Core application services
  - DatabaseService: SQLite data access and management
  - AuthService: User authentication and session management
  - NotificationService: Local push notifications

- **Helpers/**: Utility classes
  - PasswordHasher: Secure password hashing and verification
  - Value converters for UI binding

## Distribution

### Android Distribution

- **Direct APK Distribution**: Generate a signed APK and share directly with users
  ```
  dotnet publish -f net8.0-android -c Release
  ```

- **Google Play Store**: Register for a developer account ($25 one-time fee) and publish through the Play Console

### iOS Distribution

- **App Store**: Register for an Apple Developer account ($99/year) and publish through App Store Connect

### Windows Distribution

- **Microsoft Store**: Register for a developer account and publish as an MSIX package
- **Direct Distribution**: Create an installer package for direct distribution

## Recent Updates

- **Secure Password Hashing**: Implemented PBKDF2 with HMACSHA256 for secure credential storage
- **Profile Data Persistence**: Fixed issues with profile data not being displayed correctly after login
- **App Identifiers**: Updated app identifiers for distribution (com.yoursoul.app)
- **Cross-Platform Improvements**: Enhanced compatibility across all supported platforms

## Roadmap

- Cloud backend integration for multi-device synchronization
- Push notifications for real-time messaging
- Enhanced matching algorithm based on user preferences
- Location-based matching
- Media sharing in chat

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Thanks to the .NET MAUI team for the amazing cross-platform framework
- Icons and images from [Unsplash](https://unsplash.com/) and [Flaticon](https://www.flaticon.com/)
- Special thanks to all contributors and testers