import {
    Avatar,
    Box,
    Card,
    Code,
    DataList,
    Flex, Heading,
    IconButton, Link,
    Table,
    Text,
    TextField,
} from "@radix-ui/themes";
import {CopyIcon, MailIcon, PhoneIcon} from "lucide-react";
import ModifiedCard from "../components/ModifiedCard";
import LookupField from "../components/LookupField";
import BackButton from "../components/BackButton";

export function ContactDetailPage() {
    return (
        <Box flexGrow="1" p="6">
            <BackButton/>
            <Flex gap="3" direction="column" wrap="wrap">
                <Card style={{ flex: 1, minWidth: "100%" }} variant="ghost" mb="4">
                    <Flex direction="row" gap="8" wrap="wrap">
                        <Card style={{ flex: 1, minWidth: 320 }} variant="surface" size="3" mb="6">
                            <DataList.Root size="3">
                                <DataList.Item align="center">
                                    <DataList.Label>Firstname</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        <TextField.Root
                                            style={{ width: "100%" }}
                                            disabled
                                            type="text"
                                            value="Jonas"
                                        />
                                    </DataList.Value>
                                </DataList.Item>

                                <DataList.Item align="center">
                                    <DataList.Label>Firstname</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        <TextField.Root
                                            style={{ width: "100%" }}
                                            disabled
                                            type="text"
                                            value="Schneider"
                                        />
                                    </DataList.Value>
                                </DataList.Item>

                                <DataList.Item align="center">
                                    <DataList.Label>E-Mail</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        <TextField.Root
                                            style={{ width: "100%" }}
                                            disabled
                                            type="text"
                                            value="jonas.schneider@techflow.io"
                                        >
                                            <TextField.Slot side="right">
                                                <IconButton size="1" variant="ghost">
                                                    <MailIcon height="14" width="14" />
                                                </IconButton>
                                            </TextField.Slot>
                                        </TextField.Root>
                                    </DataList.Value>
                                </DataList.Item>

                                <DataList.Item align="center">
                                    <DataList.Label>Telephone</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        <TextField.Root
                                            style={{ width: "100%" }}
                                            disabled
                                            type="text"
                                            value="+49 160 9988776"
                                        >
                                            <TextField.Slot side="right">
                                                <IconButton size="1" variant="ghost">
                                                    <PhoneIcon height="14" width="14" />
                                                </IconButton>
                                            </TextField.Slot>
                                        </TextField.Root>
                                    </DataList.Value>
                                </DataList.Item>

                                <DataList.Item align="center">
                                    <DataList.Label>Company</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                            <LookupField title="Tech Flow Solution GmbH" description="Software & IT, Germany" targetUrl="/accounts/123" iconUrl="" iconFallback="78"/>
                                    </DataList.Value>
                                </DataList.Item>

                                <DataList.Item>
                                    <DataList.Label minWidth="88px">External ID</DataList.Label>
                                    <DataList.Value style={{ flex: 1 }}>
                                        <Flex align="center" gap="2">
                                            <Code variant="ghost">u_2J89JSA4GJ</Code>
                                            <IconButton
                                                size="1"
                                                aria-label="Copy value"
                                                color="gray"
                                                variant="ghost"
                                            >
                                                <CopyIcon size="16" />
                                            </IconButton>
                                        </Flex>
                                    </DataList.Value>
                                </DataList.Item>
                            </DataList.Root>
                        </Card>
                    </Flex>
                </Card>

                <Card variant="surface" size="3" mb="6">
                    <Heading size="5" mb="3">Interactions</Heading>
                    <Table.Root>
                        <Table.Header>
                            <Table.Row>
                                <Table.ColumnHeaderCell>Title</Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>Channel</Table.ColumnHeaderCell>
                                <Table.ColumnHeaderCell>Occured At</Table.ColumnHeaderCell>
                            </Table.Row>
                        </Table.Header>
                        <Table.Body>
                            <Table.Row>
                                <Table.Cell>Follow-up zum Angebot Q4</Table.Cell>
                                <Table.Cell>E-Mail</Table.Cell>
                                <Table.Cell>2025-10-28T14:00:00Z</Table.Cell>
                            </Table.Row>
                        </Table.Body>
                    </Table.Root>
                </Card>
                <ModifiedCard/>
            </Flex>
        </Box>
    );
}