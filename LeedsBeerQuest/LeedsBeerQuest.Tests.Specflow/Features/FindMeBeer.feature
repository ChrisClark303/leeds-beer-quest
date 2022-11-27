Feature: Find Me Beer

Scenario: Fetch Nearest Beer Establishments without providing a location
	Given I do not provide a location
	When I request the nearest beers establishments
	Then the 10 establishments closest to Joseph's Well should be returned
		| Name               | Lat        | Lon        | Distance |
		| The George         | 53.8010902 | -1.5524343 | 206      |
		| Fox & Newt         | 53.800045  | -1.5594819 | 287      |
		| Veritas            | 53.8009071 | -1.5511496 | 291      |
		| San Lucus          | 53.8008728 | -1.5505636 | 330      |
		| Town Hall Tavern   | 53.7995262 | -1.5513806 | 332      |
		| Wino               | 53.7983017 | -1.5531774 | 358      |
		| The Faversham      | 53.8043518 | -1.5573227 | 369      |
		| The Victoria Hotel | 53.800808  | -1.5497911 | 381      |
		| O'Neill's          | 53.8007927 | -1.549229  | 418      |
		| The Highland       | 53.7999306 | -1.5617769 | 431      |

Scenario: Fetch Nearest Beer Establishments by providing a location
	Given I provide a latitude of 53.794569064158246 and a longitiude of -1.5475488152553165
	When I request the nearest beers establishments
	Then the 10 establishments closest to that location should be returned
		| Name                           | Lat        | Lon        | Distance |
		| The White Rose                 | 53.794857  | -1.5473708 | 34       |
		| The Scarbrough Hotel           | 53.7953644 | -1.5463839 | 117      |
		| Golf"" Cafe Bar                | 53.7934952 | -1.5478653 | 121      |
		| LIVIN'italy                    | 53.7937393 | -1.5489566 | 131      |
		| Wetherspoon's, Railway Station | 53.7956467 | -1.5485017 | 135      |
		| Spencer's                      | 53.7952538 | -1.5457584 | 140      |
		| The Hop                        | 53.7937317 | -1.5491514 | 140      |
		| The Head of Steam              | 53.7952881 | -1.5457343 | 143      |
		| Baht'ap                        | 53.7951088 | -1.5454998 | 147      |
		| The Prince of Wales            | 53.7951393 | -1.54544   | 152      |

Scenario: Fetch details of an establishment by name
	Given I provide 'The Faversham' as the name of an establishment
	When I request the establishment details
	Then all details about that establishment should be returned
	| Name          | Category    | Location              | Address                              | Phone         | Twitter      | Thumbnail                                                                | Excerpt                                                                                 | Url                           | Date                  | Tags                                                 | Value | Beer | Atmosphere | Amenities |
	| The Faversham | Pub reviews | 53.8043518,-1.5573227 | 1-5 Springfield Mount, Leeds LS2 9NG | 0113 243 1481 | thefaversham | http://leedsbeer.info/wp-content/uploads/2013/09/IMG_20130910_174903.jpg | Surprising little upmarket craft beer house right on the edge of the University campus. | http://leedsbeer.info/?p=1939 | 9/15/2013 10:56:44 AM | beer garden,coffee,dance floor,food,live music,sofas | 3     | 4    | 4          | 4         |


Scenario: Attempt to fetch details of an establishment that does not exist
	Given I provide 'Non-existing establishment' as the name of an establishment
	When I request the establishment details
	Then a NoContent response should be returned