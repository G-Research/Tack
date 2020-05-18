import os
import subprocess

os.chdir(os.path.dirname(__file__))
print(f"Current working directory is {os.getcwd()}")
subprocess.run(["dotnet", "msbuild", "./Tack.sln", "/t:RunToolInDocker", "/p:Configuration=Release"], check=True)
