using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace YourSoulApp.Services
{
    public class LocationService
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        public LocationService(DatabaseService databaseService, AuthService authService)
        {
            _databaseService = databaseService;
            _authService = authService;
        }

        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                // Check if location services are enabled
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                    {
                        System.Diagnostics.Debug.WriteLine("Location permission not granted");
                        return null;
                    }
                }

                // Get location with timeout
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(15));
                Location? location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Got location: {location.Latitude}, {location.Longitude}");

                    // Update the current user's location in the database
                    if (_authService.IsLoggedIn() && AuthService.CurrentUser != null)
                    {
                        var currentUser = AuthService.CurrentUser;
                        currentUser.Latitude = location.Latitude;
                        currentUser.Longitude = location.Longitude;
                        await _databaseService.SaveUserAsync(currentUser);
                        System.Diagnostics.Debug.WriteLine("Updated user location in database");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Location returned null");
                }

                return location;
            }
            catch (FeatureNotSupportedException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Location feature not supported: {ex.Message}");
                return null;
            }
            catch (FeatureNotEnabledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Location feature not enabled: {ex.Message}");
                return null;
            }
            catch (PermissionException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Location permission error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to get location: {ex.Message}");
                return null;
            }
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula to calculate distance between two points on Earth
            const double earthRadius = 6371; // Earth's radius in kilometers

            // Convert latitude and longitude from degrees to radians
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earthRadius * c;

            return distance; // Distance in kilometers
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task<List<Models.User>> GetNearbyUsersAsync(double maxDistance)
        {
            if (!_authService.IsLoggedIn() || AuthService.CurrentUser == null)
                return new List<Models.User>();

            var currentUser = AuthService.CurrentUser;

            // Get all users from the database
            var allUsers = await _databaseService.GetAllUsersAsync();

            // Filter users based on distance and preferences
            var nearbyUsers = allUsers.Where(u =>
                u.Id != currentUser.Id && // Not the current user
                IsMatchingGenderPreference(currentUser, u) && // Matches gender preference
                IsWithinAgePreference(currentUser, u) && // Matches age preference
                CalculateDistance(currentUser.Latitude, currentUser.Longitude, u.Latitude, u.Longitude) <= maxDistance // Within distance
            ).ToList();

            return nearbyUsers;
        }

        private bool IsMatchingGenderPreference(Models.User currentUser, Models.User otherUser)
        {
            // Check if the other user's gender matches the current user's preference
            if (currentUser.InterestedIn == "Both")
                return true;

            return currentUser.InterestedIn == otherUser.Gender;
        }

        private bool IsWithinAgePreference(Models.User currentUser, Models.User otherUser)
        {
            // Check if the other user's age is within the current user's preference range
            return otherUser.Age >= currentUser.MinAgePreference &&
                   otherUser.Age <= currentUser.MaxAgePreference;
        }
    }
}
