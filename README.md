Currently used as a bridge between lighting program (grandma2) and gridnode serializer (HNode)
It takes original ArtNet packet, overrides predefined channels of objects and then sending it to the gridnode

I set HNode port to 6455 so it won't take ArtNet packets from lighting program and instead my bridge will write them
This bridge by default listens on 6454 and sends to 6455.

This is a very early project so things can break, let me know in issues if something doesn't work right.

ArtNet plugin used in this repository is https://github.com/sugi-cho/ArtNet.Unity