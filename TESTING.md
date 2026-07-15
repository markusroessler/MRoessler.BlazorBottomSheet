## Manual Tests
### Android
- see instructions in [VirtualKeyboardSample.razor](Sample.RazorComponents/Pages/VirtualKeyboardSample.razor)

## Run webkit tests on Fedora
**Create container from image**
```shell
sudo systemctl enable --now docker    
sudo systemctl enable --now docker.socket
sudo docker run -it --user "$(id -u):$(id -g)" -v $(pwd):/MRoessler.BlazorBottomSheet:Z ghcr.io/markusroessler/github.actions.workflows-maui-linux:main
```

**Start existing container**
```shell
sudo docker start -ai container_id
```

**Run tests in container**
```shell
cd MRoessler.BlazorBottomSheet
export NUGET_MROESSLER_GITHUB_PWD="ghp_XXXX"
dotnet build -f:net10.0
pwsh Sample.WebApp/bin/Debug/net10.0/playwright.ps1 install --with-deps webkit
dotnet test -f:net10.0 --settings ci-webkit.runsettings
```
