type FormattedDateProps = {
  value: string;
};

export default function FormattedDate({ value }: FormattedDateProps) {
  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return <span>Invalid date</span>;
  }

  return <span>{date.toLocaleString()}</span>;
}
