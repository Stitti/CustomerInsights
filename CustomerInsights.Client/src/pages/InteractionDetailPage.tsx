import {Box, Badge, Card, Code, DataList, Flex, IconButton, Link, Text} from "@radix-ui/themes";
import {CopyIcon, XIcon} from "lucide-react";


export function InteractionDetailPage() {
    return (
        <Box flexGrow="1" p="6">
        <Flex gap="9" direction="column" wrap="wrap">
            <Card style={{ flex: 1, minWidth: "100%" }} variant="ghost"  mb="4">
                <Flex direction="row" gap="8" wrap="wrap">
                    <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                        <DataList.Root size="3">
                            <DataList.Item align="center">
                                <DataList.Label>Title</DataList.Label>
                                <DataList.Value>Supportanfrage zu Rechnungskorrektur</DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">Contact</DataList.Label>
                                <DataList.Value>
                                    <Link href="">Anna Müller</Link>
                                </DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">Account</DataList.Label>
                                <DataList.Value>
                                    <Link href="">Müller Consulting GmbH</Link>
                                </DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">External ID</DataList.Label>
                                <DataList.Value>
                                    <Flex align="center" gap="2">
                                        <Code variant="ghost">u_2J89JSA4GJ</Code>
                                        <IconButton
                                            size="1"
                                            aria-label="Copy value"
                                            color="gray"
                                            variant="ghost"
                                        >
                                            <CopyIcon size="16"/>
                                        </IconButton>
                                    </Flex>
                                </DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">Channel</DataList.Label>
                                <DataList.Value>Email</DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">Thread</DataList.Label>
                                <DataList.Value>
                                    <Link href=""></Link>
                                </DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">Occurred At</DataList.Label>
                                <DataList.Value>2025-10-30T09:15:00Z</DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label minWidth="88px">Text</DataList.Label>
                                <DataList.Value>
                                    <Text wrap="pretty">
                                        I’ve been a loyal customer of your company for more than three years, but my recent experience has left me quite frustrated. I ordered two monitors for my home office and was promised delivery within five business days. However, the package arrived almost two weeks later, and no one from your team informed me about the delay. When I finally received the products, one of the screens had visible scratches and the other wouldn’t even turn on.

                                        Considering the high price I paid, I expected much better quality control. The packaging also looked like it had been opened before, which makes me wonder if I received a returned item. I reached out to your support department three times over the past week, but each time I was told someone would “get back to me soon.” No one ever did. It’s been extremely disappointing to deal with such slow and unhelpful service.

                                        On top of that, the invoice I received lists an additional shipping fee that was never mentioned during checkout. I tried to correct it through your online portal, but the usability of your website is honestly terrible — buttons don’t work, and the chat assistant just loops me back to the FAQ page.

                                        I really hope someone takes this seriously and resolves both the product and billing issues immediately. I’ve spent too much time and money on this already, and I’m running out of patience.
                                    </Text>
                                </DataList.Value>
                            </DataList.Item>
                        </DataList.Root>
                    </Card>
                    <Card style={{ flex: 1, maxWidth: "20vw" }} variant="surface" size="3" mb="6">
                        <DataList.Root>
                            <DataList.Item align="center">
                                <DataList.Label minWidth="88px">Status</DataList.Label>
                                <DataList.Value>
                                    <Badge color="jade" variant="soft" radius="full">
                                        Analyzed
                                    </Badge>
                                </DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label>Sentiment</DataList.Label>
                                <DataList.Value>NEGATIVE</DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label>Urgency</DataList.Label>
                                <DataList.Value>MEDIUM</DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label>Emotions</DataList.Label>
                                <DataList.Value>
                                    <Flex gap="2" wrap="wrap">
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Frustration</Text>
                                        </Badge>
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Disappointment</Text>
                                        </Badge>
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Confusion</Text>
                                        </Badge>
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Concern</Text>
                                        </Badge>
                                    </Flex>
                                </DataList.Value>
                            </DataList.Item>
                            <DataList.Item>
                                <DataList.Label>Aspects</DataList.Label>
                                <DataList.Value>
                                    <Flex gap="2" wrap="wrap">
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Lieferzeit</Text>
                                        </Badge>
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Preis</Text>
                                        </Badge>
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Support</Text>
                                        </Badge>
                                        <Badge variant="solid" color="indigo">
                                            <IconButton style={{ height: "13px", width: "13px" }}>
                                                <XIcon/>
                                            </IconButton>
                                            <Text>Qualität</Text>
                                        </Badge>
                                    </Flex>
                                </DataList.Value>
                            </DataList.Item>
                        </DataList.Root>
                    </Card>
                </Flex>
            </Card>
        </Flex>
        </Box>
    )
}