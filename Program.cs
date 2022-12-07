using System;
using GeometriaComputacional;

var selection = args.Length > 0 ? args[0] : "final";
var selectedController = 
    ControllerSelector.Select(selection);
View view = new View();
view.Controller = selectedController;
view.Run();