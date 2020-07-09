import 'package:flutter/material.dart';
import 'package:flutter/rendering.dart';

import 'page/mainPage.dart';
import 'page/settingsPage.dart';
import 'page/connectPage.dart';
import 'page/createPage.dart';
import 'page/gamePage.dart';
import 'page/transferPage.dart';

main()
{
	runApp(MyApp());
}

class MyApp extends StatelessWidget
{
	@override
	Widget build(BuildContext context)
	{
		debugPaintSizeEnabled = false;
		
		return MaterialApp(
			title: "Virtual Bank",
			theme: ThemeData.dark(),
			home: MainPage(),
			routes: <String, WidgetBuilder> {
				"/main":     (context) => MainPage(),
				"/create":   (context) => CreatePage(),
				"/settings": (context) => SettingsPage(),
				"/connect":  (context) => ConnectPage(),
				"/game":     (context) => GamePage(),
				"/transfer": (context) => TransferPage()
			},
		);
	}
}