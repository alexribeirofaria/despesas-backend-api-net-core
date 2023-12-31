# Defina o diretório do projeto (onde o arquivo docker-compose.yml está localizado)
$projectDirectory = ".\despesas-backend-api-net-core"

# Comando para realizar o build
$buildCommand = "dotnet build -restore"

# Verifique se o parâmetro -w foi passado
if ($args -contains "-w") {
    # Se o parâmetro -w foi passado, configure o comando para usar 'dotnet watch run'
    $startCommand = "dotnet watch run --project $projectDirectory"
}
else {
    # Caso contrário, use o comando padrão 'dotnet run'
    $startCommand = "dotnet run --project $projectDirectory"
}

# Execute o comando de build
Invoke-Expression $buildCommand

# Verifique se o build foi bem-sucedido
if ($LASTEXITCODE -eq 0) {
    # Se o build for bem-sucedido, execute o comando para iniciar a aplicação em segundo plano
    Start-Process "http://localhost:42535/swagger"; Invoke-Expression $startCommand
    
}
else {
    Write-Host "Falha ao construir a aplicação."
}
