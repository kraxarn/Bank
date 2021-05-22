import 'package:flutter/material.dart';
import 'dart:math';

class GamePage extends StatelessWidget
{
	var _autoRoll = true;
	
	List<int> _roll()
	{
		var random = Random();
		return [random.nextInt(6) + 1, random.nextInt(6) + 1];
	}
	
	Widget _getDiceImage(index)
	{
		return Padding (
			padding: EdgeInsets.all(2.0),
			child: Image(
				image: AssetImage("assets/ui/dice$index.png"),
				width: 32.0,
			)
		);
	}
	
	_textRow(text) =>
		Row(
			children: <Widget>[
				Text(text)
			],
		);
	
	_rollDice(context)
	{
		// Roll
		var dice = _roll();
		
		// Temporary/current roll
		var d = List<int>.from(dice);
		
		//region Auto-roll enabled
		if (_autoRoll)
		{
			// Store all rolls
			var rolls = List<int>();
			rolls.addAll(dice);
			
			// Roll while we're getting doubles
			while (d[0] == d[1] && rolls.length < 6)
			{
				d = _roll();
				rolls.addAll(d);
			}
			
			// Display result
			var title = "You rolled ${rolls.fold(0, (prev, cur) => prev + cur)}!";
			var message = <Widget>[];
			
			// Add all dice rolls
			for (var i = 0; i < rolls.length; i += 2)
				message.add(Row(
					children: <Widget>[
						_getDiceImage(rolls[i]),
						_getDiceImage(rolls[i + 1]),
					]
				));
			
			// Add double message
			if (rolls.length == 4)
				message.add(_textRow("Double!"));
			else if (rolls.length > 4)
			{
				if (d[0] == d[1])
					message.add(_textRow("3 doubles!"));
				else
					message.add(_textRow("2 doubles!"));
			}
			
			_showDiceDialog(context, title, message);
		}
		//endregion
		//region Auto-roll disabled
		else
		{
		
		}
		//endregion
	}
	
	_showDiceDialog(context, title, message)
	{
		showDialog(
			context: context,
			builder: (context) {
				return AlertDialog(
					title: Text(title),
					content: Column(
						mainAxisSize: MainAxisSize.min,
						children: message,
					),
					actions: <Widget>[
						FlatButton(
							child: Text("Neat!"),
							onPressed: () {
								Navigator.of(context).pop();
							},
						)
					],
				);
			}
		);
	}
	
	
	@override
	Widget build(BuildContext context)
	{
		return Scaffold(
			appBar: AppBar(
				title: Text("Game"),
				automaticallyImplyLeading: false,
			),
			floatingActionButton: FloatingActionButton(
				child: Icon(Icons.casino),
				onPressed: () {
					_rollDice(context);
				},
			),
			body: Padding(
				padding: EdgeInsets.all(16.0),
				child: Column(
					children: <Widget>[
						// You card
						Card(
							child: Column(
								children: <Widget>[
									// Title
									ListTile(
										title: Text(
											"You",
											style: Theme.of(context).textTheme.title,
										),
									),
									// View over self
									ListTile(
										leading: Image(
											image: AssetImage("assets/avatar/cat.png")
										),
										title: Text("OnePlus 7 Pro"),
										subtitle: Text("\$1.5k"),
										trailing: Row(
											mainAxisSize: MainAxisSize.min,
											children: <Widget>[
												IconButton(
													icon: Icon(Icons.remove),
													onPressed: () {},
													color: Color.fromARGB(255, 244, 67, 54)
												),
												IconButton(
													icon: Icon(Icons.add),
													onPressed: () {
														Navigator.of(context).pushNamed("/transfer");
													},
													color: Color.fromARGB(155, 76, 175, 80)
												),
											]
										)
									),
									// Bottom padding
									ButtonBarTheme(
										data: ButtonBarThemeData(),
										child: ButtonBar()
									)
								],
							),
						),
						// Players card
						Card(
							child: Column(
								children: <Widget>[
									// Title
									ListTile(
										title: Text(
											"Players",
											style: Theme.of(context).textTheme.title,
										),
									),
									// View over others
									ListTile(
										onTap: () {},
										leading: Image(
											image: AssetImage("assets/avatar/dog.png")
										),
										title: Text("iPhone XI Max Plus"),
										subtitle: Text("\$15")
									),
									// Bottom padding
									ButtonBarTheme(
										data: ButtonBarThemeData(),
										child: ButtonBar()
									)
								],
							),
						)
					],
				)
			),
		);
	}
}