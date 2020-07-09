import 'dart:math';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

class TransferPage extends StatelessWidget
{
	var _context;
	
	_buttonTheme(child) =>
		ButtonTheme(
			shape: ContinuousRectangleBorder(),
			materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
			height: 72.0,
			child: child,
		);
		
	_button(text, color)
	{
		return _buttonTheme(
			FlatButton(
				color: color,
				child: Text(
					text,
					style: Theme.of(_context).textTheme.title,
				),
				onPressed: () {},
			)
		);
	}
	
	_iconButton(icon, color)
	{
		return _buttonTheme(
			FlatButton(
				color: color,
				child: Icon(icon),
				onPressed: () {},
			)
		);
	}
	
	_emptyButton(color)
	{
		return _buttonTheme(
			Container(
				color: color,
			)
		);
	}
	
	@override
	Widget build(BuildContext context)
	{
		_context = context;
		return Scaffold(
			appBar: AppBar(
				title: Text("Transfer to Doggo"),
			),
			body: Align(
				alignment: Alignment.bottomCenter,
				child: Column(
					children: <Widget>[
						// Transfer display
						Expanded(
							child: Padding(
								padding: EdgeInsets.all(16.0),
								child: Column(
									mainAxisAlignment: MainAxisAlignment.center,
									children: [
										Row(
											mainAxisAlignment: MainAxisAlignment.center,
											children: <Widget>[
												Column(
													mainAxisAlignment: MainAxisAlignment.center,
													children: <Widget>[
														Text(
															"You",
															style: Theme.of(context).textTheme.title
														),
														Text("\$1.5k")
													],
												),
												Padding(
													padding: EdgeInsets.all(16.0),
													child: Icon(Icons.arrow_forward)
												),
												Column(
													mainAxisAlignment: MainAxisAlignment.center,
													children: <Widget>[
														Text(
															"Doggo",
															style: Theme.of(context).textTheme.title
														),
														Text("\$15")
													],
												),
											],
										),
										
									]
								)
							),
						),
						// Total money display
						Padding(
							padding: EdgeInsets.all(0.0),
							child: Row(
								children: <Widget>[
									_button("K", null),
									Expanded(
										child: Padding(
											padding: EdgeInsets.all(8.0),
											child: Column(
												children: [
													Text(
														"\$1,500",
														style: Theme.of(context).textTheme.subtitle,
													),
													Text(
														"\$1500",
														style: Theme.of(context).textTheme.title,
													)
												]
											)
										)
									),
									_button("M", null)
								]
							)
						),
						// Money keypad
						Table(
							children: [
								TableRow(
									children: [
										_button("7", Colors.grey[900]),
										_button("8", Colors.grey[900]),
										_button("9", Colors.grey[900]),
										_iconButton(Icons.backspace, Colors.grey[600])
									]
								),
								TableRow(
									children: [
										_button("4", Colors.grey[900]),
										_button("5", Colors.grey[900]),
										_button("6", Colors.grey[900]),
										_iconButton(Icons.send, Colors.grey[600]),
									]
								),
								TableRow(
									children: [
										_button("1", Colors.grey[900]),
										_button("2", Colors.grey[900]),
										_button("3", Colors.grey[900]),
										_emptyButton(Colors.grey[600]),
									]
								),
								TableRow(
									children: [
										_button(".",  Colors.grey[900]),
										_button("0",  Colors.grey[900]),
										_button("00", Colors.grey[900]),
										_emptyButton(Colors.grey[600])
									]
								)
							],
						)
					]
				),
			)
		);
	}
}