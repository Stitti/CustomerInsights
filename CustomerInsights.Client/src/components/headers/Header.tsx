import {Card, Flex, IconButton, Select, Text} from "@radix-ui/themes";
import {RefreshCcw, SaveIcon, SquarePenIcon, TrashIcon} from "lucide-react";
import React, {JSX} from "react";

interface Props {
    title?: string,
    showTimeInterval?: boolean,
    showSave?: boolean,
    showEdit?: boolean,
    showDelete?: boolean,
    showRefresh?: boolean,
}

export default function Header(props: Props): JSX.Element {
    return (
        <>
            <Card style={{ flex: 1, minWidth: "100%" }} mb="4">
                <Flex direction="row" style={{ justifyContent: "space-between" }}>
                    {props.title && (
                        <Text size="5">{props.title}</Text>
                    )}
                    {!props.title && props.showTimeInterval && (
                        <Select.Root defaultValue="total">
                            <Select.Trigger variant="soft" style={{width:'10%'}} />
                            <Select.Content variant="soft">
                                <Select.Item value="total">Total</Select.Item>
                                <Select.Item value="this_week">This week</Select.Item>
                                <Select.Item value="this_month">This month</Select.Item>
                                <Select.Item value="this_year">This year</Select.Item>
                                <Select.Separator />
                                <Select.Item value="last_week">Last week</Select.Item>
                                <Select.Item value="last_month">Last month</Select.Item>
                                <Select.Item value="last_year">Last year</Select.Item>
                            </Select.Content>
                        </Select.Root>
                    )}

                    <Flex direction="row" gap="4" wrap="wrap">
                        {props.title && props.showTimeInterval && (
                            <Select.Root defaultValue="total">
                                <Select.Trigger variant="soft" style={{width:'155px'}} />
                                <Select.Content variant="soft">
                                    <Select.Item value="total">Total</Select.Item>
                                    <Select.Item value="this_week">This week</Select.Item>
                                    <Select.Item value="this_month">This month</Select.Item>
                                    <Select.Item value="this_year">This year</Select.Item>
                                    <Select.Separator />
                                    <Select.Item value="last_week">Last week</Select.Item>
                                    <Select.Item value="last_month">Last month</Select.Item>
                                    <Select.Item value="last_year">Last year</Select.Item>
                                </Select.Content>
                            </Select.Root>
                        )}

                        {props.showSave && (
                            <IconButton variant="soft">
                                <SaveIcon size="20" />
                            </IconButton>
                        )}
                        {props.showEdit&& (
                            <IconButton variant="soft">
                                <SquarePenIcon size="20" />
                            </IconButton>
                        )}
                        {props. showDelete && (
                            <IconButton variant="soft">
                                <TrashIcon size="20" />
                            </IconButton>
                        )}

                        {props.showRefresh && (
                            <IconButton variant="soft">
                                <RefreshCcw size="20" />
                            </IconButton>
                        )}
                    </Flex>
                </Flex>
            </Card>
        </>
    )
}