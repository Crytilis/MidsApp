# MidsReborn API (MidsApp)

The MidsReborn API, also known as MidsApp, is responsible for allowing users to share character builds from the MidsReborn application. It is also utilized by the MidsBot, a Discord bot for searching and retrieving builds.

## Installation

To use the MidsReborn API, follow these steps:

1. Clone the repository to your local machine.
2. Ensure you have the necessary dependencies installed.
3. Build the project using your preferred IDE or command-line tool.

## Usage

### Endpoints

The MidsReborn API provides the following endpoints:

- `POST /build/submit`: Submits a new build record to the database.
- `PATCH /build/update/{shortcode}`: Updates an existing build record identified by its shortcode.
- `GET /build/download/{shortcode}`: Downloads a build file using a unique code.
- `GET /build/schema/{shortcode}`: Retrieves build data for a specific schema identified by a code.
- `GET /build/redirect-to-schema/{shortcode}`: Redirects to a build-specific schema based on a unique code.
- `GET /build/image/{shortcode}.{extension}`: Retrieves an image associated with a build, identified by code and file extension.
- `GET /build/list`: Retrieves a list of build records based on search criteria.

### Authentication

The MidsReborn API does not require authentication for most endpoints. However, certain endpoints may require authorization based on future updates.

## Examples

### Submitting a Build

```http
POST /build/submit
Content-Type: application/json

{
  "name": "My Awesome Build",
  "archetype": "Tanker",
  "primary": "Fire",
  "secondary": "Ice",
  "buildData": "BASE64_ENCODED_BUILD_DATA",
  "imageData": "BASE64_ENCODED_IMAGE_DATA"
}
```

### Updating a Build

```http
PATCH /build/update/{shortcode}
Content-Type: application/json

{
  "name": "Updated Build Name",
  "description": "Updated description of the build."
}
```

### Downloading a Build

```http
GET /build/download/{shortcode}
```

### Retrieving Build Data for Schema

```http
GET /build/schema/{shortcode}
```

### Retrieving an Image

```http
GET /build/image/{shortcode}.png
```

### Searching Builds

```http
GET /build/list
Content-Type: application/json

"Tanker,Fire,Ice"
```

###

## Configuration
```json
{
  "ApiSettings": {
    "BaseUrl": "REPLACE_WITH_YOUR_URL",
    "AppProtocol":  "REPLACE_WITH_APP_PROTOCOL"
  },
  "MongoDb": {
    "ConnectionString": "REPLACE_WITH_MONGODB_CONNECTION_STRING",
    "DatabaseName": "REPLACE_WITH_YOUR_DATABASE_NAME"
  },
  "JwtSettings": {
    "AccessKey": "REPLACE_WITH_ACCESS_KEY",
    "RefreshKey": "REPLACE_WITH_REFRESH_KEY",
    "Audience": "REPLACE_WITH_YOUR_URL",
    "Issuer": "REPLACE_WITH_YOUR_URL",
    "AccessExpires": 7,
    "RefreshExpires": 14
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Contributing

Contributions to the MidsReborn API are welcome! Feel free to submit bug reports, suggest enhancements, or contribute code via pull requests.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For any questions or support, please reach out here via issues or discussions.
