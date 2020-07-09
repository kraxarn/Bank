import 'package:flutter/material.dart';

class ConnectPage extends StatelessWidget
{
	@override
	Widget build(BuildContext context)
	{
		return Scaffold(
			// Blue 700
			backgroundColor: Color.fromARGB(255, 25, 118, 210),
			body: Center(
				child: Column(
					mainAxisSize: MainAxisSize.min,
					children: <Widget>[
						Image(
							image: AssetImage("/assets/ui/logo.png"),
						),
						CircularProgressIndicator(),
						SizedBox(
							height: 16.0,
						),
						Text(
							"Connecting to server...",
							style: Theme.of(context).textTheme.title,
						)
					]
				)
			)
		);
	}
}