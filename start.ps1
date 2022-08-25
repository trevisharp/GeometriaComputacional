clear
if ($args.Length -le 0)
{
    dotnet run -c Release
}
else
{
    dotnet run $args[0] -c Release
}