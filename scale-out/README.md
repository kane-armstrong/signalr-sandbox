# Running this manually

1. `docker pull redis`
2. `docker run -p 6379:6379 --name my-redis -d redis`
3. Terminal tab > change directory to Hub > `dotnet run`
4. Terminal tab > change directory to Client.GroupOne > `dotnet run`
5. Terminal tab > change directory to Client.GroupTwo > `dotnet run`
6. Poke it with Postman

# Running with Docker

1. `docker-compose build`
2. `docker-compose up`
3. Poke it with Postman