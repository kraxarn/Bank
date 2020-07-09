import 'dart:io';

import 'package:flutter/material.dart';

class CreatePage extends StatelessWidget
{
	
	@override
	Widget build(BuildContext context)
	{
		return Scaffold(
			appBar: AppBar(
				title: Text("Create Game")
			),
			floatingActionButton: FloatingActionButton(
				child: Icon(Icons.play_arrow),
				onPressed: () {
					Navigator.of(context).pop();
					Navigator.of(context).pushNamed("/game");
				},
			),
			body: Padding(
				padding: EdgeInsets.all(16.0),
				child: Column(
					children: [
						// Options card
						Card(
							child: Column(
								children: [
									// Title
									ListTile(
										title: Text(
											"Options",
											style: Theme.of(context).textTheme.title
										),
									),
									// Starting money entry
									ListTile(
										title: Padding(
											padding: EdgeInsets.only(
												bottom: 0.0
											),
											child: TextField(
												keyboardType: TextInputType.numberWithOptions(
													signed: true,
													decimal: true
												),
												decoration: InputDecoration(
													hintText: "1.5",
													labelText: "Starting Money",
													border: OutlineInputBorder(
														borderRadius: BorderRadius.all(Radius.circular(8.0))
													)
												)
											)
										),
										trailing: Padding(
											padding: EdgeInsets.only(
												top: 16.0
											),
											child: DropdownButton<String>(
												value: "K",
												onChanged: (value) {},
												items: [" ", "K", "M"].map((value) {
													return DropdownMenuItem<String>(
														value: value,
														child: Text(value)
													);
												}).toList()
											)
										)
									),
									// Lazy fix for bottom padding
									ButtonTheme.bar(
										child: ButtonBar(),
									)
								]
							)
						),
						// Players card
						Card(
							child: Column(
								children: <Widget>[
									ListTile(
										title: Text(
											"Players",
											style: Theme.of(context).textTheme.title,
										),
									),
									SizedBox(
										height: 260.0,
										child: ListView(
											children: <Widget>[
												ListTile(
													title: Text("OnePlus 7 Pro"),
													subtitle: Text("You"),
													leading: Image(
														image: AssetImage("assets/avatar/cat.png"),
													),
												)
											],
										)
									),
								],
							),
						)
					],
				)
			)
		);
	}
}