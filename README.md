# HotelListingAPI

## Pull and Start MSSQL Server locally
### Pull
```
docker pull mcr.microsoft.com/mssql/server
```
### Start
Choose a strong password or the server will not start
```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Str0ngPa$$w0rd' -p 1400:1433 -d mcr.microsoft.com/mssql/server
```