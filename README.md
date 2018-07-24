# Scraper

Mystic Springs: the fetcher makes a request for each date in the range of dates, from today's date to the last date with viewable availability on the site (this date gets manually input and updated when necessary). The dates are fetched in random order and with a short stall in between requests so as not to look suspicious.
The request consists of the check in date and the check out date being the next day. There is no way to view individual room numbers, so there is no way to account for potential availability conflicts where there may be availability night by night, but in actuality a room change would be required.

Summit Penthouses: the fetcher only makes one request, for the date range from today's date to the last date with viewable availability on the site (this date gets manually input and updated when necessary).
The parser counts a date as available if it is not only available on one night, but is also available on either the next night or the previous night as well, since Cirrus requires a 2 night minimum and this rids of availability conflicts where there may be one individual unit available one night and a different individual unit available the next night; there is not actually availability because the stay would require a room change.

Fire Mountain: the fetcher only makes one request, for the date range from today's date to the last date with viewable availability on the site (this date gets manually input and updated when necessary).
The parser counts a date as available if it is not only available on one night, but is also available on either the next night or the previous night as well, since Cirrus requires a 2 night minimum and this rids of availability conflicts where there may be one individual unit available one night and a different individual unit available the next night; there is not actually availability because the stay would require a room change.

Banff Boundary: the fetcher only makes one request, for the date range from today's date to the last date with viewable availability on the site (this date gets manually input and updated when necessary).
The parser counts a date as available if it is not only available on one night, but is also available on either the next night or the previous night as well, since Cirrus requires a 2 night minimum and this rids of availability conflicts where there may be one individual unit available one night and a different individual unit available the next night; there is not actually availability because the stay would require a room change.

Silver Creek:

Big White:

Silver Star:

Silver Star Vance Creek: