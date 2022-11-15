Feature: Find Me Beer

Scenario: Fetch Nearest Beer Establishments without providing a location
	Given I do not provide a location
	When I request the nearest beers establishments
	Then the 10 establishments closest to Joseph's Well should be returned
		|        Name        | Lat        | Lon        | Distance |
		| The George         | 53.8010902 | -1.5524343 | 0.128 |
		| Fox & Newt         | 53.800045  | -1.5594819 | 0.178 |
		| Veritas            | 53.8009071 | -1.5511496 | 0.181 |
		| San Lucus          | 53.8008728 | -1.5505636 | 0.205 |
		| Town Hall Tavern   | 53.7995262 | -1.5513806 | 0.206 |
		| Wino               | 53.7983017 | -1.5531774 | 0.222 |
		| The Faversham      | 53.8043518 | -1.5573227 | 0.229 |
		| The Victoria Hotel | 53.800808  | -1.5497911 | 0.237 |
		| O'Neill's          | 53.8007927 | -1.549229  | 0.26  |
		| The Highland       | 53.7999306 | -1.5617769 | 0.268 |

		