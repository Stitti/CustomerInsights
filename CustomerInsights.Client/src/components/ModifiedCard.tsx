import {Card, DataList, Flex, IconButton, Link, Select, Text, TextField} from "@radix-ui/themes";
import {MailIcon, RefreshCcw, SaveIcon, TrashIcon} from "lucide-react";

export default function ModifiedCard () {
    return (
        <Card style={{ flex: 1, minWidth: "100%" }}  mb="4">
            <Flex direction="row" style={{justifyContent: "space-between", marginLeft: "1rem", marginRight: "1rem"}} >
                <DataList.Root size="3">
                    <DataList.Item align="center">
                        <DataList.Label>Created At</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <Text>01.11.2025</Text>
                        </DataList.Value>
                    </DataList.Item>

                    <DataList.Item align="center">
                        <DataList.Label>Created By</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <Link>Dataverse Interface</Link>
                        </DataList.Value>
                    </DataList.Item>
                </DataList.Root>

                <DataList.Root size="3">
                    <DataList.Item align="center">
                        <DataList.Label>Modified At</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <Text>06.11.2025</Text>
                        </DataList.Value>
                    </DataList.Item>

                    <DataList.Item align="center">
                        <DataList.Label>Modified By</DataList.Label>
                        <DataList.Value style={{ flex: 1 }}>
                            <Link>Max Mustermann</Link>
                        </DataList.Value>
                    </DataList.Item>
                </DataList.Root>
            </Flex>
        </Card>
    )
}